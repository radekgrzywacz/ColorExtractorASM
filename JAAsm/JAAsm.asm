; BitmapConverter.asm
.model flat, stdcall
.code

; Deklaracja eksportowanej funkcji
public ImageToBitArray
ImageToBitArrayASM proc bitmap:DWORD, bits:DWORD, width:DWORD, height:DWORD
    push ebp
    mov ebp, esp
    push ebx
    push esi
    push edi

    mov esi, bitmap        ; źródłowy wskaźnik do bitmap data
    mov edi, bits         ; docelowa tablica bajtów
    mov ecx, width        ; szerokość
    mov edx, height       ; wysokość

    ; Oblicz stride (szerokość * 3, wyrównana do 4 bajtów)
    mov eax, ecx
    imul eax, 3          ; szerokość * 3 (RGB)
    add eax, 3
    and eax, not 3       ; wyrównanie do 4 bajtów
    mov ebx, eax         ; ebx = stride

    ; Liczniki
    xor eax, eax         ; licznik y
outer_loop:
    cmp eax, edx
    jge done
    
    push eax
    xor ecx, ecx         ; licznik x
    
inner_loop:
    cmp ecx, [ebp+12]    ; porównaj z szerokością
    jge next_row
    
    ; Kopiuj RGB (odwrócona kolejność BGR -> RGB)
    mov al, [esi+2]      ; Red
    mov [edi], al
    mov al, [esi+1]      ; Green
    mov [edi+1], al
    mov al, [esi]        ; Blue
    mov [edi+2], al
    
    add esi, 3           ; następny piksel źródłowy
    add edi, 3           ; następny piksel docelowy
    inc ecx
    jmp inner_loop

next_row:
    pop eax
    add esi, ebx         ; przejdź do następnego wiersza
    inc eax
    jmp outer_loop

done:
    pop edi
    pop esi
    pop ebx
    mov esp, ebp
    pop ebp
    ret 16              ; wyczyść 4 parametry ze stosu (4 * 4 = 16 bajtów)
ImageToBitArray endp

end