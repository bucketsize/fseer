open Futil

type cpu_info = {
    mutable arch: string;
    mutable usage: float;
    mutable cs: int64;
    mutable cz: int64;
}

let ci = {arch="foo"; usage=0.0; cs=0L; cz=0L}

let cputime () =
    let line = List.nth (read_file_lines "/proc/stat") 0 in
    let cputs =
        string_find_all line "[0-9]+" 
            |> List.map Int64.of_string
    in
    List.fold_left (fun s x -> Int64.add s x) 0L cputs, List.nth cputs 3 

let info ()  = 
    let css, czz = cputime () in
    (* let () = Printf.printf "cputime ts: (%s, %s) - (%s, %s)\n" *)
    (*     (Int64.to_string css) (Int64.to_string czz) *)
    (*     (Int64.to_string ci.cs) (Int64.to_string ci.cz) *)
    (* in *)
    let ds = Int64.to_float (Int64.sub css ci.cs) and
        dz = Int64.to_float (Int64.sub czz ci.cz) in
    let cu = 1.0 -. (dz /. ds) in
    let () = ci.cs <- css in 
    let () = ci.cz <- czz in
    let () = ci.usage <- cu in
    ci
