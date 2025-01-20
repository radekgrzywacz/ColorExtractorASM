.data    
    ; Mask for blue channel (0xFF for each pixel)
    mask_blue  dd 000000FFh, 000000FFh, 000000FFh, 000000FFh
               dd 000000FFh, 000000FFh, 000000FFh, 000000FFh

    ; Mask for green channel (0xFF00 for each pixel)
    mask_green dd 0000FF00h, 0000FF00h, 0000FF00h, 0000FF00h
               dd 0000FF00h, 0000FF00h, 0000FF00h, 0000FF00h

    ; Mask for red channel (0xFF0000 for each pixel)
    mask_red   dd 00FF0000h, 00FF0000h, 00FF0000h, 00FF0000h
               dd 00FF0000h, 00FF0000h, 00FF0000h, 00FF0000h
; Input:
; rcx - pointer to pixel data
; rdx - number of pixels to process
; r8  - pointer to result (sum of R+G+B values)

.code
CalculateAverageBrightness proc
    push rbp
    mov rbp, rsp
    
    ; Initialize accumulator to zero
    vxorps ymm0, ymm0, ymm0    ; sum accumulator
    
    ; Process 32 bytes (8 pixels) at a time
    mov rax, rdx
    shr rax, 3                  ; divide by 8 to get number of full vector operations
    
    xor r9, r9                  ; counter
    
process_loop:
    cmp r9, rax
    jge cleanup
    
    ; Load 8 pixels (32 bytes) into YMM1
    vmovdqu ymm1, ymmword ptr [rcx + r9*4]
    
    ; Extract and add R, G, B components
    ; Assuming BGRA format (each component is 1 byte)
    vpand ymm2, ymm1, [mask_blue]   ; Extract blue
    vpsrld ymm2, ymm2, 0            ; No shift needed for blue
    
    vpand ymm3, ymm1, [mask_green]  ; Extract green
    vpsrld ymm3, ymm3, 8            ; Shift right by 8
    
    vpand ymm4, ymm1, [mask_red]    ; Extract red
    vpsrld ymm4, ymm4, 16           ; Shift right by 16
    
    ; Add components
    vpaddd ymm2, ymm2, ymm3         ; Add green to blue
    vpaddd ymm2, ymm2, ymm4         ; Add red
    
    ; Add to accumulator
    vpaddd ymm0, ymm0, ymm2
    
    inc r9
    jmp process_loop

cleanup:
    ; Horizontal add within ymm0 to get final sum
    vextracti128 xmm1, ymm0, 1      ; Extract upper 128 bits
    vpaddd xmm0, xmm0, xmm1         ; Add upper to lower
    
    vphaddd xmm0, xmm0, xmm0        ; Horizontal add within 128 bits
    vphaddd xmm0, xmm0, xmm0
    
    ; Store result
    vmovd dword ptr [r8], xmm0
        
    pop rbp
    ret

CalculateAverageBrightness endp

CalculateTemperature proc
    push rbp
    mov rbp, rsp
    
    ; Initialize accumulator to zero
    vxorps ymm0, ymm0, ymm0    ; sum accumulator
    
    ; Process 8 pixels at a time
    mov rax, rdx
    shr rax, 3                  ; divide by 8 to get number of full vector operations
    
    xor r9, r9                  ; counter
    
process_loop:
    cmp r9, rax
    jge cleanup
    
    ; Load 8 pixels into YMM1
    vmovdqu ymm1, ymmword ptr [rcx + r9*4]
    
    ; Extract blue components
    vpand ymm2, ymm1, [mask_blue]   ; Extract blue
    vpsrld ymm2, ymm2, 0            ; No shift needed for blue
    
    ; Extract red components
    vpand ymm3, ymm1, [mask_red]    ; Extract red
    vpsrld ymm3, ymm3, 16           ; Shift right by 16
    
    ; Calculate R-B
    vpsubd ymm4, ymm3, ymm2         ; Subtract blue from red
    
    ; Add to accumulator
    vpaddd ymm0, ymm0, ymm4
    
    inc r9
    jmp process_loop
    
cleanup:
    ; Horizontal add within ymm0 to get final sum
    vextracti128 xmm1, ymm0, 1      ; Extract upper 128 bits
    vpaddd xmm0, xmm0, xmm1         ; Add upper to lower
    
    vphaddd xmm0, xmm0, xmm0        ; Horizontal add within 128 bits
    vphaddd xmm0, xmm0, xmm0
    
    ; Store result
    vmovd dword ptr [r8], xmm0
        
    pop rbp
    ret
CalculateTemperature endp


CalculateDominantChannel proc
    push rbp
    mov rbp, rsp

    ; Save the fifth parameter pointer
    mov r10, qword ptr [rbp + 48]  ; Access 5th parameter (blue sum pointer)

    ; Initialize accumulators to zero
    vxorps ymm0, ymm0, ymm0    ; red sum
    vxorps ymm1, ymm1, ymm1    ; green sum
    vxorps ymm2, ymm2, ymm2    ; blue sum

    ; Process 8 pixels at a time
    mov rax, rdx
    shr rax, 3                  ; divide pixel count by 8

    xor r11, r11               ; loop counter (using r11 instead of r9 since r9 is parameter)

process_loop:
    cmp r11, rax
    jge cleanup

    ; Load 8 pixels into YMM3
    vmovdqu ymm3, ymmword ptr [rcx + r11*4]

    ; Extract and sum each channel (BGRA format)
    vpand ymm4, ymm3, [mask_blue]   ; Extract blue
    vpaddd ymm2, ymm2, ymm4         ; Add to blue sum

    vpand ymm4, ymm3, [mask_green]  ; Extract green
    vpsrld ymm4, ymm4, 8
    vpaddd ymm1, ymm1, ymm4         ; Add to green sum

    vpand ymm4, ymm3, [mask_red]    ; Extract red
    vpsrld ymm4, ymm4, 16
    vpaddd ymm0, ymm0, ymm4         ; Add to red sum

    inc r11
    jmp process_loop

cleanup:
    ; Calculate final sums and store in respective pointers
    ; Red sum
    vextracti128 xmm3, ymm0, 1
    vpaddd xmm0, xmm0, xmm3
    vphaddd xmm0, xmm0, xmm0
    vphaddd xmm0, xmm0, xmm0
    vmovd dword ptr [r8], xmm0      ; Store red sum to first pointer

    ; Green sum
    vextracti128 xmm3, ymm1, 1
    vpaddd xmm1, xmm1, xmm3
    vphaddd xmm1, xmm1, xmm1
    vphaddd xmm1, xmm1, xmm1
    vmovd dword ptr [r9], xmm1      ; Store green sum to second pointer

    ; Blue sum
    vextracti128 xmm3, ymm2, 1
    vpaddd xmm2, xmm2, xmm3
    vphaddd xmm2, xmm2, xmm2
    vphaddd xmm2, xmm2, xmm2
    vmovd dword ptr [r10], xmm2     ; Store blue sum to third pointer

    pop rbp
    ret
CalculateDominantChannel endp


end