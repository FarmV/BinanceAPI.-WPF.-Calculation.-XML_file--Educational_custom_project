![Image](https://i.imgur.com/HMXemHL.jpeg)

### Краткое описание проекта
* [Educational Custom Project](https://win10tweaker.ru/forum/topic/%d0%bf%d0%b0%d1%80%d1%81%d0%b8%d0%bd%d0%b3-json-%d0%bf%d0%be-api-%d0%b8-%d0%b7%d0%b0%d0%bf%d0%b8%d1%81%d1%8c-%d0%b2-%d1%84%d0%b0%d0%b9%d0%bb%d1%8b) - означает что проект реализован по некоммерческому заказу, и преследует учебные цели.

### Реализуймая задача:
1. Получить данные от BinanceAPI в Json из 2 источников.
2. Преобразовать их в объекты .Net для манипуляций.
3. Предоставить пользовательский интерфейс для расчётов по определённой формуле.
4. Выгрузить или изменить файлы XML в соответствии c шаблоном, и полученными результатами вычислений, так как выходные файлы являются входными для другой программы.


### Основные возможности:
1. Расчет значений по формуле.
2. Выгрузка около 200 файлов.
3. Установка новой директории для выгрузки.
4. Открытие указанной директории в Explorer.
---
## Неполный предметный указатель по проекту
| Что | Описание | Ссылки на примеры кода |
| --- | --- | --- |
| ReactiveCommand | Использование реактивных команд из ReactiveUI | [ViewMode1](https://github.com/FarmV/BinanceAPI.-WPF.-Calculation.-XML_file--Educational_custom_project/blob/f0f71ce79db53341b153ddc6320025d41995fd24/AppPars/Main/MainWindowViewModel.cs#L38-L41) [ViewMode2](https://github.com/FarmV/BinanceAPI.-WPF.-Calculation.-XML_file--Educational_custom_project/blob/f0f71ce79db53341b153ddc6320025d41995fd24/AppPars/Main/MainWindowViewModel.cs#L121-L136) [MainWindow](https://github.com/FarmV/BinanceAPI.-WPF.-Calculation.-XML_file--Educational_custom_project/blob/f0f71ce79db53341b153ddc6320025d41995fd24/AppPars/Main/MainWindow.xaml.cs#L92-L117) |
| JsonConverter and JSON DOM | Преобразование Json ответа, с усечением, в объект | [ReqestLogic](https://github.com/FarmV/BinanceAPI.-WPF.-Calculation.-XML_file--Educational_custom_project/blob/f0f71ce79db53341b153ddc6320025d41995fd24/AppPars/ReqestLogic.cs#L20-L135) |
| LINQ XML DOM | Преобразование исходного XML файла по шаблону | [DataManagement](https://github.com/FarmV/BinanceAPI.-WPF.-Calculation.-XML_file--Educational_custom_project/blob/f0f71ce79db53341b153ddc6320025d41995fd24/AppPars/DataManagement.cs#L214-L231) |
|DwmEnableBlurBehindWindow| Установка прозрачности для окна (несмотря на название и документацию?) | [MainWindow1](https://github.com/FarmV/BinanceAPI.-WPF.-Calculation.-XML_file--Educational_custom_project/blob/master/AppPars/Main/MainWindow.xaml.cs#L200-L220) [MainWindow2](https://github.com/FarmV/BinanceAPI.-WPF.-Calculation.-XML_file--Educational_custom_project/blob/master/AppPars/Main/MainWindow.xaml.cs#L148-L162)
