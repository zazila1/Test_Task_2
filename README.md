
Июль 2019
## Тестовое задание 2:
Разработать программу, которая по клику на сцену добавляет прямоугольники разного цвета (или текстуры) в то место, где был произведен клик. По двойному клику по прямоугольнику, он удаляется. Прямоугольники можно перетаскивать по сцене. Между прямоугольниками можно создавать/удалять связь (визуально - линия). 
 
Размер прямоугольников фиксирован (соотношение сторон 2:1). 
Цвет создаваемого прямоугольника выбирается случайным образом в момент его создания. 
Выбор способа создания связи между прямоугольниками осуществляется разработчиком. 
Прямоугольники не могут перекрывать друг друга ни при перетаскивании, ни при создании. 
Для упрощения, прямоугольник не создается, если область на сцене, по которой произведен клик, слишком мала для размещения в ней прямоугольника. 
При перетаскивании связанных прямоугольников связь сохраняется. 
При удалении прямоугольника связь удалятся. 
Количество созданных прямоугольников не ограничено. 
 
## Комментарии по реализации:

<a href="https://wmpics.pics/pm-DL4Z.html"><img src="https://wmpics.pics/dm-DL4Z.gif" /></a>

Потратил около ~20ч+, т.к. некоторые аспекты делал впервые. Так же пришлось покопаться со связями.
 
В GameManager можно изменять размер первоначального пула объектов, если пул кончается, то создаются дополнительные объекты.

В префабе Box можно переключить тип движения при перемещении (через силу или через скорость) + сделать небольшие настройки.
