//
// experiment with syntactic sugar to clean things up even more
// with computation expressions to hide some of the plumbing
//
#if INTERACTIVE
#load "AST.fs"
#load "AST_Utils.fs"
#load "Operators.fs"
#load "Traversals.fs"
#load "Interpreter.fs"
#load "CompExpr.fs"
#endif

open AST
open AST_Utils
open Operators
open Traversals
open Interpreter
open CompExpr

let tester = environment {
  // set up some bindings in the environment and
  // acquire handles for use in building expressions
  let! a = newvar "a" 2.7 
  let! b = newvar "b" 42.0
  let! x = newvar "x" 6.
  let! y = newvar "y" 7.

  let! pi = newconst 3.1415

  // return an expression assembled in the context of the
  // environment established above.
  return pi .* ((a .+ b) ./ (x .* y))
  }

let result = processM tester
