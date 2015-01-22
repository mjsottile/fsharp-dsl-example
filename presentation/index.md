- title : FsReveal
- description : Introduction to FsReveal
- author : Karlkim Suwanmongkol
- theme : night
- transition : default

***

## Basic DSL Concepts in F#

### Portland F# Meetup, January 2015

***

## Matthew Sottile

- Principal Investigator - Galois, Inc.
- Founder/President - Sailfan Research, Inc.
- Adjunct Faculty - Washington State University, EECS

***

### Overview:

- DSLs
  - Design space
  - Embedding vs standalone
- FSharp building blocks
- Hands on

> **Note**: This represents my take on DSLs.  Other perspectives on the topic exist.

***

### Domain Specific Languages

- Specialized
  - E.g.: Hardware config; crypto; build systems
- Heavy leverage other languages
  - Avoid reinvention/reimplementation of critical infrastructure

---

#### Aspects of DSL design

- Data elements
- Operators
- Combinators
- Semantics
- Syntax

---

#### Embedded or not?

- Embedded (EDSL)
  - Reside within syntax of a host language
- Standalone
  - Provide specialized, independent syntax

- If you've used FAKE for FSharp, you've used an EDSL

> More often than not, I lean towards EDSLs

---

#### EDSLs

- All but the syntax and *syntax* error reporting
- Embedding requires working out all of the other details

- Syntax is an exercise in parsing
- Error reporting is a subtler, trickier matter

---

#### EDSLs and error reporting

- EDSL resides in host language
  - Syntax errors are host language syntax errors

- Confusing to DSL users to see host language detail
  - Especially if sugar provided by DSL intends to hide host

***

### Why is FP so nice for (E)DSLs?

- Rich type systems
- Very good symbolic programming facilities
- Lots of runtime and language infrastructure for free

---

#### Type systems

- Rich, extensible type systems
  - H-M type inference
  - Not the same as type "inferrence" like C++ auto keyword

> Rich, strong type system === lightweight formal verification for free!

---

#### Symbolic programming

- Manipulation of structures
- Many built in abstractions for this
  - E.g., pattern matching; algebraic data types
  - Very expressive, very compact
- Strong types
  - Keep manipulations safe

---

#### Infrastructure for free

- Closures 
- Memory management + GC
- First-class functions
- Pattern matching

***

### FSharp Building Blocks

#### (Non-exhaustive list)

- Discriminated unions (aka, algebraic data types)
- Pattern matching
- Infix operators
- Computation Expressions

Others worth noting: active patterns, code quotations

---

#### Discriminated Unions

    type BOp = BPlus | BMinus | BTimes | BDivide
    type Constant = FConst of float | IConst of int
    type Expr = BinOp of BOp * Expr * Expr
              | Const of Constant

- Extremely compact
- Traversed via pattern matching
- Type checked at compile time
  - ...including non-exhaustive tests, which classical enums don't provide

> Those four lines above define the AST for basic arithmetic expressions

---

#### Pattern matching

    let rec eval e =
      match e with
      | BinOp (op, e1, e2) -> binEval op (eval e1) (eval e2)
      | Constant c -> match c with
                      | FConst f -> f
                      | IConst i -> float i
      | _ -> failwith "Unexpected Expr encountered"

- Matches instance of type being eval'd
- Deconstructs value associated with instance into named variables
- Type checker can catch non-exhaustive matches

- *Much* more sophisticated than a fancy switch statement

---

#### Infix operators

    let inline (.+) e1 e2 = BinOp (BPlus,e1,e2)
    let fc f = Constant (FConst f)
    let ic i = Constant (IConst i)
    let x = (fc 4.0) .+ (fc 5.0)

- This is key to the embedding - custom, compact operators in host language that assemble AST
- These are heavily used in EDSLs in most functional languages

---

#### Computation expressions

    let env = newEnv()
    let env1 = addToEnv env "x" 1.0
    let env2 = addToEnv env1 "y" 2.0

- Often want to hide bookkeeping while assembing EDSL programs
  - E.g., defining an environment for evaluating DSL programs
- This is not terribly pleasant to write
- Computation expressions allow one to hide the environment updates while still being *purely functional*
  - ...instead of cheating with globals and/or using mutable state.

---

#### Computation expressions

    let tester = environment {
      let! a = newvar "a" 2.7 
      let! b = newvar "b" 42.0
      let! x = newvar "x" 6.
      let! y = newvar "y" 7.

      let! pi = newconst 3.1415

      return pi .* ((a .+ b) ./ (x .* y))
    }


- That is much cleaner, eh?

***

### Hands-on

* Two choices:
  * Classic DSL example: arithmetic expressions
  * Building on prior meetups: a DSL for basic graphics

---

#### Expression DSL

* https://github.com/mjsottile/fsharp-dsl-example

---

#### Expression DSL : Core

* Organized as:
  * AST.fs : The AST nodes
  * AST_Utils.fs : Helpers (e.g., pretty printer)
  * Operators.fs : Things to operate on the AST
  * Traversals.fs : Things to traverse and manipulate the AST
  * Interpreter.fs : Interpreter for AST structures with environments
  * CompExpr.fs : Computational expression support code

---

#### Expression DSL : Demonstrators

* Demonstrated via:
  * Expressions.fsx : Basic work with ASTs
  * Sugar1.fsx : Working with interpreter and introducing some syntactic sugar
  * Sugar2.fsx : More syntactic sugar via computation expressions

---

#### Expression DSL : Exercises

* Exercises (in order of estimated complexity):
  * *Extend AST*: represent other nodes (sqrt, sin, cos, etc..)
  * *New traversals*: constant inlining
  * *Transformations*: double negation removal, basic differentiation
  * *Parsing*: interpret strings like "1+5*6" by instantiating AST and evaling
  * *Computation expressions*: come up with a more compact embedding

---

#### Graphics DSL

* Same repo: https://github.com/mjsottile/fsharp-dsl-example

* Based on FSharp Fractals Dojo

* Exercises
  * More operators for more graphical primitives
  * 2D constructive solid geometry DSL
    * Elements: circles, squares, etc..
    * Operators: addition, subtraction, xor, etc..
  * Interpreter for alternative back-ends
    * SVG? PNG?

***

## Thank you!

