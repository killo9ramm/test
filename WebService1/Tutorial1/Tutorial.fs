
// Файл учебника по F#
//
// Этот файл содержит примеры кода для изучения
// основ языка F#.  
//
// Дополнительные сведения о F# см. на http://fsharp.net
// 
// См. более разнообразную коллекцию примеров на F#:
//     http://go.microsoft.com/fwlink/?LinkID=124614
//
// Содержание:
//   - Простые вычисления
//   - Функции целых чисел  
//   - Кортежи 
//   - Логические значения
//   - Строки
//   - Списки
//   - Массивы
//   - Дополнительные коллекции
//   - Функции
//   - Типы: объединения
//   - Типы: записи
//   - Типы: классы
//   - Типы: интерфейсы
//   - Типы: классы с реализациями интерфейсов
//   - Печать

// открытие некоторых стандартных пространств имен

open System

// Простые вычисления
// ---------------------------------------------------------------
// Примеры некоторых простых вычислений. Инструкции по документированию кода
// с помощью комментариев "///". Установите указатель на любую ссылку на переменную,
// чтобы увидеть ее описание.

/// Очень простое константное целое
let int1 = 1

/// Другое очень простое константное целое
let int2 = 2

/// Добавление двух целых
let int3 = int1 + int2

// Функции на целых  
// ---------------------------------------------------------------

/// Функция на целых
let f x = 2*x*x - 5*x + 3

/// Результат простого вычисления
let result = f (int3 + 4)

/// Другая функция на целых
let increment x = x + 1

/// Вычисление факториала целого
let rec factorial n = if n=0 then 1 else n * factorial (n-1)

/// Вычисление наибольшего общего делителя двух целых
let rec hcf a b =                       // примечание: 2 параметра, разделенные пробелами
    if a=0 then b
    elif a<b then hcf a (b-a)           // примечание: 2 аргумента, разделенные пробелами
    else hcf (a-b) b
    // примечание: аргументы функции обычно разделяются пробелами
    // примечание: "let rec" определяет рекурсивную функцию

      
// Кортежи
// ---------------------------------------------------------------

// Простой кортеж целых
let pointA = (1, 2, 3)

// Простой кортеж целого, строки и числа с плавающей точкой двойной точности
let dataB = (1, "fred", 3.1415)

/// Функция, которая переставляет два числа в кортеже
let Swap (a, b) = (b, a)

// Логические значения
// ---------------------------------------------------------------

/// Простое логическое значение
let boolean1 = true

/// Другое простое логическое значение
let boolean2 = false

/// Вычисление нового логического значения с помощью операторов and, or и not
let boolean3 = not boolean1 && (boolean2 || false)

// Строки
// ---------------------------------------------------------------

/// Простая строка
let stringA  = "Hello"

/// Другая простая строка
let stringB  = "world"

/// "Здравствуй, мир", вычисленная с помощью соединения строк
let stringC  = stringA + " " + stringB

/// "Здравствуй, мир", вычисленная с помощью функции библиотеки .NET
let stringD = String.Join(" ",[| stringA; stringB |])
  // Попробуйте повторить ввод предыдущей строки, чтобы увидеть технологию IntelliSense в действии
  // Заметьте, что Ctrl-J на неполных идентификаторах реактивирует IntelliSense

// Функциональные списки
// ---------------------------------------------------------------

/// Пустой список
let listA = [ ]           

/// Список с тремя целыми
let listB = [ 1; 2; 3 ]     

/// Список с тремя целыми, заметьте :: является операцией "cons"
let listC = 1 :: [2; 3]    

/// Вычисления суммы списка целых с помощью рекурсивной функции
let rec SumList xs =
    match xs with
    | []    -> 0
    | y::ys -> y + SumList ys

/// Сумма списка
let listD = SumList [1; 2; 3]  

/// Список целых от 1 до 10 включительно
let oneToTen = [1..10]

/// Квадраты первых 10 целых
let squaresOfOneToTen = [ for x in 0..10 -> x*x ]


// Изменяемые массивы
// ---------------------------------------------------------------

/// Создание массива
let arr = Array.create 4 "hello"
arr.[1] <- "world"
arr.[3] <- "don"

/// Вычисление длины массива с помощью экземплярного метода на объекте массива
let arrLength = arr.Length        

// Извлечение подмассива с помощью slice-нотации
let front = arr.[0..2]


// Дополнительные коллекции
// ---------------------------------------------------------------

/// Словарь с целыми ключами и строковыми значениями
let lookupTable = dict [ (1, "One"); (2, "Two") ]

let oneString = lookupTable.[1]

// См. примеры для некоторых других общих структур данных:
//   System.Collections.Generic
//   Microsoft.FSharp.Collections
//   Microsoft.FSharp.Collections.Seq
//   Microsoft.FSharp.Collections.Set
//   Microsoft.FSharp.Collections.Map

// Функции
// ---------------------------------------------------------------

/// Функция, возводящая в квадрат свой входной параметр
let Square x = x*x              

// Сопоставление функции по списку значений
let squares1 = List.map Square [1; 2; 3; 4]
let squares2 = List.map (fun x -> x*x) [1; 2; 3; 4]

// Конвейеры
let squares3 = [1; 2; 3; 4] |> List.map (fun x -> x*x) 
let SumOfSquaresUpTo n = 
  [1..n] 
  |> List.map Square 
  |> List.sum

// Типы: объединения
// ---------------------------------------------------------------

type Expr = 
  | Num of int
  | Add of Expr * Expr
  | Mul of Expr * Expr
  | Var of string
  
let rec Evaluate (env:Map<string,int>) exp = 
    match exp with
    | Num n -> n
    | Add (x,y) -> Evaluate env x + Evaluate env y
    | Mul (x,y) -> Evaluate env x * Evaluate env y
    | Var id    -> env.[id]
  
let envA = Map.ofList [ "a",1 ;
                        "b",2 ;
                        "c",3 ]
             
let expT1 = Add(Var "a",Mul(Num 2,Var "b"))
let resT1 = Evaluate envA expT1


open System 

type Book =
  val title : string
  val author : string
  val publishDate : DateTime

  new (t, a, pd) = {
    title = t
    author = a
    publishDate = pd
  }


let k=new Book("12", "2", DateTime.Now);

  
// Типы: записи
// ---------------------------------------------------------------

type Card = { Name  : string;
              Phone : string;
              Ok    : bool }
              
let cardA = { Name = "Alf" ; Phone = "(206) 555-0157" ; Ok = false }
let cardB = { cardA with Phone = "(206) 555-0112"; Ok = true }
let ShowCard c = 
  c.Name + " Phone: " + c.Phone + (if not c.Ok then " (unchecked)" else "");;



// Типы: классы
// ---------------------------------------------------------------

/// Двумерный массив
type Vector2D(dx:float, dy:float) = 
    // Предварительно вычисленная длина вектора
    let length = sqrt(dx*dx + dy*dy)
    /// Смещение по оси X
    member v.DX = dx
    /// Смещение по оси Y
    member v.DY = dy
    /// Длина вектора
    member v.Length = length
    // Смещение вектора на константу
    member v.Scale(k) = Vector2D(k*dx, k*dy)
    
let m=Vector2D(1.0,1.0)
m.Scale(2.0)
// Типы: интерфейсы
// ---------------------------------------------------------------

type IPeekPoke = 
    abstract Peek: unit -> int
    abstract Poke: int -> unit
    abstract istate : int

              
// Типы: классы с реализациями интерфейсов
// ---------------------------------------------------------------

/// Элемент интерфейса, подсчитывающий, сколько раз он был щелкнут
type Widget(initialState:int) = 
    /// Внутреннее состояние элемента интерфейса
    let mutable state = initialState

    member x.x_state = initialState

    member x.Hello = "hi"
    // Реализация интерфейса IPeekPoke
    interface IPeekPoke with 
        member x.Poke(n) = state <- state + n
        member x.Peek() = x.x_state 
        member x.istate=x.x_state
    /// Элемент интерфейса был щелкнут?
    member x.HasBeenPoked = (state <> 0)

let widget = Widget(12) :> IPeekPoke
let widget1 = Widget(12)


//widget1.
//
//widget.

widget.Poke(4)
let peekResult = widget.Peek()

              
// Печать
// ---------------------------------------------------------------

// Печать целого
printfn "peekResult = %d" peekResult 

// Универсальная печать результата с помощью %A
printfn "listC = %A" listC
printfn "listC = %A" "asd"
printfn "listC = %A" 15
