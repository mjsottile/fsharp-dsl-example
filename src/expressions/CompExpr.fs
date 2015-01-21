//
// use of basic computation expression to provide syntactic sugar over
// environment creation and creation of variables for corresponding
// AST nodes to use in expression creation.
//
// see http://bit.ly/1AKVS6l for basic state code used for below
//
module CompExpr

open AST
open AST_Utils
open Operators
open Interpreter

let (>>=) x f = fun s0 -> let a,s = x s0 in f a s
let returnS a = fun s -> a,s

type StateBuilder() =
  member m.Bind(x, f) = x >>= f
  member m.Return a = returnS a

let state = new StateBuilder()
let getState = (fun s -> s, s)
let setState s = (fun _ -> (),s)

// custom stuff layered on top of state
let newvar v x = state {
  let vv = Variable v
  let vv2 = (v, fval x)
  let! s = getState
  do! setState (vv2::s)
  return vv
  }

let newconst x = state {
  let v = Constant x
  return v
  }
  
let Execute m s = m s

let environment = state

let processM me = let expression,environment = Execute me []
                  let env = Map.ofList environment
                  (expression, pretty expression,
                   environment, interpret env expression)
