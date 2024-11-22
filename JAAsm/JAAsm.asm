section .text
global ProcessPixelsASM

; Procedure prototype:
; Input:
;   ESI -> pointer to region array
;   EDI -> pointer to pixelProcessor procedure
;   EDX -> pointer to resultAggregator procedure
;   EAX -> initialValue
; Output:
;   EAX -> final result
; Preserves all registers except EAX

ProcessPixelsASM:
    push    ebp
    mov     ebp, esp
    push    ebx
    push    ecx
    push    edx
    push    esi
    push    edi

    ; Save initial value
    push    eax         ; Save on stack as working value

process_loop:
    ; Check if we have at least 4 more bytes
    mov     ebx, [esi+3]    ; Try to read 4th byte ahead
    jz      done            ; If we can't, we're done

    ; Get RGB values
    xor     ebx, ebx        ; Clear ebx for byte loads
    mov     bl, [esi]       ; Blue
    push    ebx
    mov     bl, [esi+1]     ; Green
    push    ebx
    mov     bl, [esi+2]     ; Red
    push    ebx

    ; Call pixelProcessor
    call    edi             ; pixelProcessor(r, g, b)
    add     esp, 12         ; Clean up parameters

    ; Save pixelResult
    push    eax

    ; Get current aggregatedResult
    mov     ebx, [ebp-4]    ; Load current aggregated result

    ; Call resultAggregator
    push    eax             ; pixelResult
    push    ebx             ; aggregatedResult
    call    edx             ; resultAggregator(aggregatedResult, pixelResult)
    add     esp, 8          ; Clean up parameters

    ; Store new aggregated result
    mov     [ebp-4], eax

    ; Move to next pixel
    add     esi, 4
    jmp     process_loop

done:
    ; Get final result into eax
    mov     eax, [ebp-4]

    ; Restore registers
    pop     edi
    pop     esi
    pop     edx
    pop     ecx
    pop     ebx
    pop     ebp
    ret