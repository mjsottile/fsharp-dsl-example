open System.Text

type StringFragment =
  | Empty
  | Fragment of string
  | Concat of StringFragment * StringFragment
  override x.ToString() =
    let rec flatten frag (sb : StringBuilder) =
      match frag with
        | Empty -> sb
        | Fragment(s) -> sb.Append(s)
        | Concat(s1,s2) -> sb |> flatten s1 |> flatten s2
    (StringBuilder() |> flatten x).ToString()

type StringFragmentBuilder() =
  member x.Zero() = Empty
  member x.Yield(v) = Fragment(v)
  member x.YieldFrom(v) = v
  member x.Combine(l, r) = Concat(l, r)
  member x.Delay(f) = f()
  member x.For(s, f) =
    Seq.map f s |> Seq.reduce (fun l r -> x.Combine(l,r))

let buildstring = StringFragmentBuilder()

let x = buildstring {
  yield "a"
  yield "b"
  }

// see: http://www.navision-blog.de/2009/10/23/using-monads-in-fsharp-part-i-the-state-monad/

let (>>=) x f = fun s0 -> let a,s = x s0 in f a s

let returnS a = fun s -> a,s

type StateTestBuilder() =
  member m.Bind(x, f) = x >>= f
  member m.Return a = returnS a

let state = new StateTestBuilder()

type Variable = Var of string

type VariableRef = VarRef of Variable * int

let getState = (fun s -> s, s)

let setState s = (fun _ -> (),s)

let newvar v x = state {
  let vv = Var v
  let vv2 = VarRef (vv,x)
  let! s = getState
  do! setState (vv2::s)
  return vv
  }
  
let Execute m s = m s

let tester = state {
  let! x = newvar "foo" 1 
  let! y = newvar "bar" 2
  return ()
  }

let testM t = Execute t []
