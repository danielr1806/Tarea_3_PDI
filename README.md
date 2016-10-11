#**Tarea 3 PDI**

**Desarrollo**

La aplicacion fue desarollada en el lenguaje C# en Visual Studio 2015 para el  
sistema operativo Windows. Para ejecutar basta con abrir el archivo ejecutable (.exe)  
o abrir el archivo de formato .sln y ejecutar desde Visual Studio

**Entrada y Salida**

El formato de la imagen de entrada puede ser: **BMP**, **JPG** y **PNG**.   
La salida en tal caso que quiera guardar la imagen, será una imagen formato  
**BMP** de 32bits, y de no querer guardarla, la imagen estará en un picturebox  
de la interfaz gráfica.


**Controles**

Requerimientos de la tarea 2:   

Una vez ejecutado el programa deberá cargar la imagen de su preferencia,  
para aplicar el Negativo, Ecualización, Espejo Vertical, Espejo Horizontal, Zoom in, Zoom out y Calcular el histograma constara de presionar su botón   
correspondiente. En el caso de la Umbralizacion tendrá que ingresar el valor a   
comparar y presionar su respectivo botón. Para el brillo y el contraste debe de   
utilizar el slider para escoger el valor de su preferencia y presionar el botón   
aceptar de la correspondiente operación. Para el escalamiento, deberá escoger  
un nuevo alto y ancho después precione su correspondiente botón.  
Para cancelar los cambios hechos, presione el botón *Cancelar Cambios* y para  
aplicarlos, presione el botón *Aplicar Cambios* . Para guardar o abrir un archivo  
precione en el menu horizontal el botón *Archivo*   

Requerimientos de la tarea 3:
En las pestañas de la aplicación escoja la opción de su preferencia y el tamaño  
del filtro. En caso de querer un filtro personalizado, se adjuntaron unos  
ejemplos en la carpeta " Ejemplos ", dichos ejemplos siguen un formato para  
que la matriz pueda ser leida correctamente.


**Informacón Adicional:**
Para el escalamiento utilizamos el algoritmo interpolación bilineal y para  
el Zoom utilizamos interpolación nearest neighbor.*  
Dependiendo del tamaño de la imagen, las operaciones pueden tardar segundos o minutos.  
Para las operaciones de Escalamiento y Zoom las imagenes tienen que ser guardadas  
y abiertas en un visualizador de imagenes apropiado.


**Integrantes:**

*Alexander Yammine V-23529986*   
*Daniel Rodriguez V-24883818*
