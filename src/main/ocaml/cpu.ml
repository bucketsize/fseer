open Base
open Stdio

type cpu_info = {
    mutable arch: string;
    mutable usage: float;
    mutable cs: int64;
    mutable cz: int64;
}

let cputime cs0 cz0 =
    let line = List.nth (read_file_lines "/proc/stat") 0 in
    let ls = String.split_on_char ' ' line in
    match ls with
    | (_::values) ->  
        let cputs =
            values 
            |> List.filter (fun x -> (x <>  "")) 
            |> List.map Int64.of_string
        in
        List.fold_left (fun s x -> Int64.add s x) 0L cputs, List.nth cputs 0 
    | [] -> cs0, cz0 

let ci = {arch="foo"; usage=0.0; cs=0L; cz=0L}

let info ()  = 
    let css, czz = cputime ci.cs ci.cz in
    let cu = 1.0 -. Int64.to_float (Int64.div (Int64.sub czz ci.cz) (Int64.sub css ci.cs)) in
    let () = ci.cs <- css in 
    let () = ci.cz <- czz in
    ci
