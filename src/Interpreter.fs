module Interpreter

open AST

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

