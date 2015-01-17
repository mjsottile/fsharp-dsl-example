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

- Principal investigator - Galois, Inc.
- Founder/President - Sailfan Research, Inc.
- Adjunct Faculty - Washington State University, EECS

***

### Overview:

- DSLs: embedded or not.
- FSharp building blocks
- Hands on

> **Note**: This represents my take on DSLs.  Other perspectives on the topic exist.

***

### Domain Specific Languages

- Purpose-built
- Specialized
- Heavy leverage other languages

---

#### Embedded or not?

- Embedded (EDSL): reside within syntax of a host language.
- Standalone: introduce specialized syntax

- If you've used FAKE for FSharp, you've used an EDSL.

> More often than not, I lean towards EDSLs.

---

#### Aspects of DSL design

- Data elements
- Operators
- Combinators
- Semantics
- Syntax

---

#### EDSLs

- All but the syntax.
- Embedding requires working out all of the details.
- Moving to standalone is an exercise in parsers.

***

### Why is FP so nice for (E)DSLs?

- Rich, extensible type systems w/ H-M type inference
  - Not the same as type "inferrence" like C++ auto keyword
- ML-derivatives are very good at symbolic programming
  - Manipulation of structures
  - Lots of syntactic sugar = very expressive, very compact
  - Strong types = keep manipulations safe
- Languages provide lots for free
  - Closures 
  - Memory management + GC
  - First-class functions
  - Pattern matching

> Rich, strong type system === lightweight formal verification for free!

***

### FSharp Building Blocks

#### (Non-exhaustive list)

- Discriminated unions (aka, algebraic data types)
- Pattern matching
- Infix operators
- Currying
- Computation Expressions

---

#### Discriminated Unions

    type BOp = BPlus | BMinus | BTimes | BDivide
    type Constant = FConst of float | IConst of int
    type Expr = BinOp of BOp * Expr * Expr
              | Const of Constant

- Extremely compact relative to classes, records, etc.
- Traversed via pattern matching.
- Type checked at compile time.
- ...including non-exhaustive tests, which classical enums don't provide.

> Those four lines above define the AST for basic arithmetic expressions.

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

- *Much* more sophisticated than a fancy switch statement.

---

#### Infix operators

    let inline (.+) e1 e2 = BinOp (BPlus,e1,e2)
    let fc f = Constant (FConst f)
    let ic i = Constant (IConst i)
    let x = (fc 4.0) .+ (fc 5.0)

- This is key to the embedding - custom, compact operators in host language that assemble AST.
- These are heavily used in EDSLs in most functional languages.

---

#### Currying

Write me

---

#### Computation expressions

    let env = newEnv()
    let env1 = addToEnv env "x" 1.0
    let env2 = addToEnv env1 "y" 2.0

- Often want to hide bookkeeping while assembing EDSL programs.
- Example: defining an environment for evaluating DSL programs.
- This is not terribly pleasant to write.
- Computation expressions allow one to hide the environment updates while still being *purely functional*.
- ...instead of cheating with globals and/or using mutable state.

---

#### Computation expressions

    let env = newEnv {
      "x" <<- 1.0;
      "y" <<- 2.0;
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

* Exercises (in order of estimated complexity):
  * *Extend AST*: represent other nodes (sqrt, sin, cos, etc..)
  * *New traversals*: constant inlining
  * *Transformations*: double negation removal, basic differentiation
  * *Parsing*: interpret strings like "1+5*6" by instantiating AST and evaling.
  * *Computation expressions*: come up with a more compact embedding.

---

#### Graphics DSL

* Same repo

* Based on FSharp Fractals Dojo

* Exercises
  * More operators for more graphical primitives
  * 2D constructive solid geometry DSL
    * Elements: circles, squares, etc..
    * Operators: addition, subtraction, xor, etc...
  * Interpreter for alternative back-ends
    * SVG? PNG?

***

## Thank you!

