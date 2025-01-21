.data    
    ; Maska dla niebieskiego 00000000 00000000 00000000 11111111
    mask_blue  dd 000000FFh, 000000FFh, 000000FFh, 000000FFh
               dd 000000FFh, 000000FFh, 000000FFh, 000000FFh

    ; Maska dla zielonego 00000000 00000000 11111111 00000000
    mask_green dd 0000FF00h, 0000FF00h, 0000FF00h, 0000FF00h
               dd 0000FF00h, 0000FF00h, 0000FF00h, 0000FF00h

    ; Maska dla czerwonego 00000000 11111111 00000000 00000000
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
    
    ; Zerowanie rejestru na sume
    vxorps ymm0, ymm0, ymm0    
    
    ; 32 bajty na raz (8 pikseli argb) => 256 bitów
    mov rax, rdx
    shr rax, 3                  ; przesuniecie o 3 pozycje, czyli podzielenie przez 8
    
    xor r9, r9                  ; zerujemy licznik
    
process_loop:

    ; Sprawdzamy czy licznik jest mniejszy od ilości pikseli
    cmp r9, rax
    jge cleanup
    
    ; Ładujemy dane do rejestru ymm1 z rcx (wskaźnik do pixeli z parametrów) + r9 (licznik pętli) * 4 (bo pixel ma 4 bajty)
    vmovdqu ymm1, ymmword ptr [rcx + r9*4]
    
    ; Wyciąganie kanałów
    vpand ymm2, ymm1, [mask_blue]   ; Niebieski
    ;vpsrld ymm2, ymm2, 0            ; No shift needed for blue
    
    vpand ymm3, ymm1, [mask_green]  ; Zielony
    vpsrld ymm3, ymm3, 8            ; Przesunięcie żeby znormalizować do zakresu 0 - 255
    
    vpand ymm4, ymm1, [mask_red]    ; Czerwony
    vpsrld ymm4, ymm4, 16           ; Znowu przesunięcie
    
    ; Dodawanie kanałów
    vpaddd ymm2, ymm2, ymm3         ; Niebieski + zielony
    vpaddd ymm2, ymm2, ymm4         ; Poprzednie + czerwony
    
    ; Dodawanie do sumy
    vpaddd ymm0, ymm0, ymm2
    
    inc r9
    jmp process_loop

cleanup:
    ; Horizontal add within ymm0 to get final sum
    vextracti128 xmm1, ymm0, 1      ; Wyciągamy mlodsze 128 bitów
    vpaddd xmm0, xmm0, xmm1         ; i starsze do nich dodajemy
    
    vphaddd xmm0, xmm0, xmm0        ; i teraz suma xmm0 = [a, b, c, d] => xmm0 = [(a + b) + (c + d), ?, ?, ?]
    vphaddd xmm0, xmm0, xmm0
    
    ; Zwracamy do r8 znowu
    vmovd dword ptr [r8], xmm0
        
    pop rbp
    ret

CalculateAverageBrightness endp

CalculateTemperature proc
    push rbp
    mov rbp, rsp
    
    ; Zerujemy akumulator
    vxorps ymm0, ymm0, ymm0    
    
    ; Przetwarzamy 8 pikseli 
    mov rax, rdx
    shr rax, 3                  ; przesuwamy o 3 zeby podzielic przez 8
    
    xor r9, r9                  ; licznik
    
process_loop:
    cmp r9, rax
    jge cleanup
    
    ; Ladujemy 8 pixeli + przesuniecie juz przerobionych
    vmovdqu ymm1, ymmword ptr [rcx + r9*4]
    
    ; Wyciagamy kanaly
    vpand ymm2, ymm1, [mask_blue]   
    vpsrld ymm2, ymm2, 0            
    
    vpand ymm3, ymm1, [mask_red]    
    vpsrld ymm3, ymm3, 16      
    
    ; Glowne dzialanie !
    vpsubd ymm4, ymm3, ymm2         ; odejmowanie czerwonego od niebieskiego!
    
    ; Dodajemy do wyniku
    vpaddd ymm0, ymm0, ymm4
    
    inc r9
    jmp process_loop
    
cleanup:
    ; Horizontal add within ymm0 to get final sum
    vextracti128 xmm1, ymm0, 1      
    vpaddd xmm0, xmm0, xmm1         ; jak wyzej
    
    vphaddd xmm0, xmm0, xmm0        ; jak wyzej
    vphaddd xmm0, xmm0, xmm0
    
    ; Zapsujemy do zwrotu
    vmovd dword ptr [r8], xmm0
        
    pop rbp
    ret
CalculateTemperature endp


CalculateDominantChannel proc
    push rbp
    mov rbp, rsp

    ; Wrzucamy piaty parametr do rejestru
    mov r10, qword ptr [rbp + 48]  ; rbp + 48 bo base pointer + 48 bajtów bo rbp - 8b + 4 parametry * 8b + 5 parametr = 48b

    ; Xorujemy rejestry zeby bylo 0
    vxorps ymm0, ymm0, ymm0    ; czerwony
    vxorps ymm1, ymm1, ymm1    ; zielony
    vxorps ymm2, ymm2, ymm2    ; niebieski

    ; 8px na raz
    mov rax, rdx
    shr rax, 3                  ; przesuniecie 3 zeby dzielic przez 8

    xor r11, r11               ; counter

process_loop:
    cmp r11, rax
    jge cleanup

    ; Ladujemy 8px
    vmovdqu ymm3, ymmword ptr [rcx + r11*4] ; rcx adres poczatkowy wskaznika, r11 licznik * 4 bo 4 bajty na pixel

    ; Ekstrakcja kanałów
    vpand ymm4, ymm3, [mask_blue]   
    vpaddd ymm2, ymm2, ymm4 
    vpaddd ymm2, ymm2, ymm4         ; Dodaj do sumy niebieskich

    vpand ymm4, ymm3, [mask_green] 
    vpsrld ymm4, ymm4, 8
    vpaddd ymm1, ymm1, ymm4         ; Dodaj do sumy zielonych

    vpand ymm4, ymm3, [mask_red]    
    vpsrld ymm4, ymm4, 16
    vpaddd ymm0, ymm0, ymm4         ; Dodaj do czerwonych

    inc r11
    jmp process_loop

cleanup:
    ; Sumuj czerwony
    vextracti128 xmm3, ymm0, 1  ; mlodsze
    vpaddd xmm0, xmm0, xmm3     ; + starsze
    vphaddd xmm0, xmm0, xmm0    ;horyzontalne dodawanie
    vphaddd xmm0, xmm0, xmm0
    vmovd dword ptr [r8], xmm0      ; zwroc na pierwszy pointer

    ; sumuj zielony
    vextracti128 xmm3, ymm1, 1  ; jak wyzej
    vpaddd xmm1, xmm1, xmm3
    vphaddd xmm1, xmm1, xmm1
    vphaddd xmm1, xmm1, xmm1
    vmovd dword ptr [r9], xmm1      ; zwroc na drugi pointer

    ; sumuj niebieski
    vextracti128 xmm3, ymm2, 1  ; jak wyzej
    vpaddd xmm2, xmm2, xmm3
    vphaddd xmm2, xmm2, xmm2
    vphaddd xmm2, xmm2, xmm2
    vmovd dword ptr [r10], xmm2     ; zwroc na trzeci pointer

    pop rbp
    ret
CalculateDominantChannel endp


end