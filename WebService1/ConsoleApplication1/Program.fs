module File1

open System


let cons (t:string)=
    System.Console.WriteLine(t)

cons "привет"
System.Console.ReadKey()

type Book =
  val title : string
  val author : string
  val publishDate : DateTime

  new (t, a, pd) = {
    title = t
    author = a
    publishDate = pd
  }

type Card = { Name  : string;
            Phone : string;
            Ok    : bool }

type Stack (ilist:'t list)=
    let mutable lis=ilist
    let mutable myInternalValue=0
    member this._list with get () = lis
    member this._list with set (value) = lis <- value
    member this.MyReadWriteProperty with get () = myInternalValue
    member this.MyReadWriteProperty with set (value) = myInternalValue <- value
    member x.Hello = "hi"
    member x.HasBeenPoked = (x._list.Length <> 0)    
//    static member myStaticValue=0
//    static member MyStaticProperty
//        with get() = myStaticValue
//        and set(value) = myStaticValue <- value











[<EntryPoint>]
let main argv=
    printfn "end"
    0
