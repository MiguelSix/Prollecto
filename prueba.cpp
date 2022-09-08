#include <iostream>
#include <stdio.h>
#include <conio.h>
float area, radio, pi, resultado;
int a, b;
// Este programa calcula el volumen de un cilindro.
void main(){
    printf("\n\n\tRadio =  ");
    scanf("%d", &radio);
    pi = 3.141592653589793;
    b = 4;
    area = pi * (radio * radio);
    b++;
    printf("\nArea = ");
    printf(area);
}