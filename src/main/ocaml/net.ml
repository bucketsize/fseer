open Futil
open Printf

type net_if = {
    intf: string;
    rx: int64; (* Bytes *)
    tx: int64;
}

type net_info = {
    intfs : net_if list
}

let string_find_all (s:string) (r:string) = 
    let rec string_find_p (s:string) (re:Str.regexp) (i:int) (acc:string list) =
        if Str.string_match re s i 
        then string_find_p s re (Str.match_end ()) ((Str.matched_string s)::acc)
        else acc
    in
    string_find_p s (Str.regexp r) 0 []

let info () =
    let intfs = 
        read_file_lines "/proc/net/dev"
        |> List.filter (fun x -> not (has_in x "Inter"))
        |> List.filter (fun x -> not (has_in x " face" ))
        |> List.map (fun x -> 
            let xs = String.split_on_char ':' x in
            let nd = String.trim (List.nth xs 0) in
            let ns = List.nth xs 1 in
            let nv = 
                string_find_all ns "(\\d+)"
                |> List.map (fun x -> Int64.of_string x)
            in
            let () = printf "%d" (List.length nv) in
            {intf = nd;  rx = List.nth nv 0; tx = List.nth nv 1})
    in
    {intfs = intfs}

