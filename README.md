# Itransition-FeedbackPlatform 
## САЙТ ДЛЯ УПРАВЛЕНИЯ РЕКОМЕНLАЦИЯМИ "ЧЁ ПОСМОТРЕТЬ/ЧЁ ПОЧИТАТЬ/ВО ЧТО ПОИГРАТЬ И Т.Д."
### Описание проекта:

Неаутентифицированным пользователи доступен только режим read-only (доступен поиск, недоступно создание обзоров, нельзя оставлять комментарии, ставить лайки и рейтинги).
Аутентифицированные пользователи имеют доступ ко всему, кроме админки. В базовом варианте админка представляет собой список пользователей (как ссылок на их страницы). 

Требуется поддерживать аутентификацию через социальные сети (Facebook, Google).

Администратор видит каждую страницу пользователя и каждый "обзор" как ее создатель (например, может отредактировать или создать от имени пользователя с его страницы новый "обзор").

На каждой странице доступен полнотекстовый поиск по сайту (результаты поиска - всегда обозоры, например, если текст найден в комментарии, что должно быть возможно, то выводится ссылка на обзор).

У каждого пользователя есть его личная страница, на которой он видит список своих обзоров (таблица с фильтраций и сортировками, возможность создать/удалить/редактировать обзор/открыть в режиме просмотра). 

Каждый обзор состоит из: названия обзора, названия произведения (см. также требования со *), "группа" (из фиксированного набора, например, "Кино", "Книги", "Игры" и т.п.), тэгов (вводится несколько тэгов, необходимо автодополнение - когда пользователь начинает вводить тэг, выпадает список с вариантами слов, которые уже вводились ранее на сайте), текста обзора (с поддержкой форматирования markdown), опциональное изображение-иллюстрация (хранение в облаке) и оценки от автора по 10-б. шкале.

Все картинки сохраняются в облаке, загружаются драг-н-дропом.

На главной странице отображаются: последние добавленные обзоры, обзоры с самыми большими оценками, облако тэгов.
Каждый пользовать может проставить "рейтинг" (1..5 звезд) произведению — средний пользовательский рейтинг отображается рядом с названием произведения везде на сайте.

Также пользователь может поставить лайк собственно самому обзору (не более 1 на обзор от 1 пользователя), эти лайки складываются по всем обзорам пользователя и отображаются рядом с именем пользователя.

Если два пользователя описываются одно кино, это никак не связано между собой.

Под обзором в режиме просмотра (или другими пользователями) в конце отображаются комментарии. Комментарии линейные, нельзя комментировать комментарий, новый добавляется только "в хвост". Автоматическая подгрузка комментариев — если у меня открыта страница с комментариями и кто-то другой добавляет новый, он автомагически появляется (возможна задержка в 2-5 секунд). 
 
Сайт должен поддерживать два оформления (темы): светлое и темное (пользователь выбирает и выбор сохраняется).

### Веб-сайт: [http://peabody28.com:5093/]
