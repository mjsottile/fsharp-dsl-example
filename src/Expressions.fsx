//
// Simple EDSL for expressions in F#
//

//
// first, establish the types to represent the AST that will be assembled
// and later traversed
//

// binary operators
type BOp = BPlus | BMinus | BTimes | BDivide

// unary operators
type UOp = UNeg

//
// discriminated union representing basic expressions.
// no precedence rules are considered - we just assume the children of
// the operators are parenthesized.
//
// e.g., (a+b)*(c+d) versus a+b*c+d.
//
type Expr = 
    | BinOp of BOp * Expr * Expr
    | UnOp of UOp * Expr
    | Variable of string
    | Constant of float

// given the AST, now we can write traversals of it.  the simplest and
// most useful for debugging is a pretty printer.

let pretty_bop o =
    match o with
    | BPlus -> "+"
    | BMinus -> "-"
    | BTimes -> "*"
    | BDivide -> "/"

let pretty_uop o =
    match o with
    | UNeg -> "-"

let rec pretty ex =
    match ex with
    | BinOp (op,l,r) -> "("+pretty l+pretty_bop op+pretty r+")"
    | UnOp (op,child) -> pretty_uop op+pretty child
    | Variable s -> s
    | Constant c -> string c

//
// an impotant part of embedding the DSL in F# is to come up with
// F# operators for assembling the AST without explicitly calling the
// consructors for the discriminated union                    
//
let inline (.+) e1 e2 = BinOp (BPlus,e1,e2)
let inline (.-) e1 e2 = BinOp (BMinus,e1,e2)
let inline (.*) e1 e2 = BinOp (BTimes,e1,e2)
let inline (./) e1 e2 = BinOp (BDivide,e1,e2)

//
// create a function called "neg" for negation
//
let neg e1 = UnOp (UNeg,e1)

//
// simple functiosn to lift constants into the AST as Constant nodes.
//
let fval (x : float) = Constant x

//
// similar function to lift strings into the AST for variable references
//
let v s = Variable s

//
// some examples
//
let test1 = let x = v "x"
            let y = v "y"
            x .+ y

let test2 = let (x,y,a) = (v "x", v "y", v "a")
            a .* (x .+ y)

//
// implement the distributive law via pattern matching
//   
let rec distrib ex =
    match ex with
    | BinOp (BTimes, e1, BinOp (o,e2,e3)) -> BinOp (o, e1 .* e2, e1 .* e3)
    | BinOp (o, e1, e2) -> BinOp (o, distrib e1, distrib e2)
    | UnOp (o, e) -> UnOp (o, distrib e)
    | _ -> ex

//
// write a simple interpreter
//

// first, define an environment type to bind variables to values
type interp_env = Map<string, Expr>

let rec interpret (e:interp_env) exp =
    let interp_bop bop e1 e2 =
        match (e1, e2) with
        | (Constant c1, Constant c2) -> let v = match bop with
                                                | BPlus -> c1 + c2
                                                | BMinus -> c1 - c2
                                                | BTimes -> c1 * c2
                                                | BDivide -> c1 / c2
                                        Constant v
        | _ -> failwith "Operands must evaluate to constants."

    let interp_uop uop e1 =
        match e1 with
        | Constant c -> let v = match uop with
                                | UNeg -> -c
                        Constant v
        | _ -> failwith "Operands must evaluate to constants."

    match exp with
    | BinOp (op, e1, e2) -> let v1 = interpret e e1
                            let v2 = interpret e e2
                            interp_bop op v1 v2
    | UnOp (op, e1) -> let v1 = interpret e e1
                       interp_uop op v1
    | Variable s -> e.[s]
    | Constant c -> Constant c

//
// do an example with our test2 above
//
let test2_env = Map.ofList [("a", fval 5.); ("x", fval 6.); ("y", fval 2.)]

// interpret test2
let result = interpret test2_env test2

// interpret test2 after the distributive law has been applied
let result2 = distrib test2 |> interpret test2_env

// some code to make assembling environments a little cleaner, hiding the
// data structure that is used to represent them.
let inline (<<-) v value = match v with
                           | Variable name -> (name, fval value)
                           | _ -> failwith "<<- cannot be used with non-variables"

let mkenv = Map.ofList 

//
// final test: showing everything together.
//
let test3 =
    // declare a set of variables
    let (a,b,x,y) = (v "a", v "b", v "x", v "y" )

    // bind the variables to some values
    let env = mkenv [
                a <<- 2.7;
                b <<- 3.14;
                x <<- 6.;
                y <<- 7.; ]

    // define an expression using these variables
    let expr = (a .+ b) ./ (x .* y)

    // pretty print it
    let pretty_string = pretty expr

    // return everything: the expression, environment,
    // pretty string, and the interpreted expression result
    (expr, env, pretty_string, interpret env expr)

//
// future ideas: clean things up even more by using computation expressions
// to hide the construction of the environment via State and possibly clean
// up the variable declarations as well
//

