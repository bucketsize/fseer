let rec get_item ls s =
    match ls with
    | (x::xs) -> 
            let k, _ = x in
            if k = s
                then Some x
                else get_item xs s
    | [] ->  None        

let rec read_lines channel acc =
    let mline = Stdio.In_channel.input_line channel in
    match mline with
        | Some line -> read_lines channel (line :: acc)
        | None -> List.rev acc

let rec apply_lines channel acc =
    let mline = Stdio.In_channel.input_line channel in
    match mline with
        | Some line -> 
                let () = acc line in
                let () = apply_lines channel acc in
                ()
        | None -> () 

let read_file_lines filename = 
    let file_channel = Stdio.In_channel.create filename in
    let lines = read_lines file_channel [] in
    let () = Stdio.In_channel.close file_channel in
    lines

let read_file_line filename = 
    let file_channel = Stdio.In_channel.create filename in
    let mline = Stdio.In_channel.input_line file_channel in
    let () = Stdio.In_channel.close file_channel in
    mline

let extract_in_between (s:string) (e:string) (line:string) =
    let is = 
        try Str.search_forward (Str.regexp_string s) line 0
        with Not_found -> -1
    in 
    let ie = 
        try Str.search_forward (Str.regexp_string e) line is
        with Not_found -> -1
    in
        if is < 0 || ie < 0 then
            None
        else
            let x = (String.sub line (is + String.length s) (ie-is-(String.length s))) in
            (* let () = Printf.printf "|%s|\n" x in *)
            Some (String.trim x)

let has_in (s:string) (m:string) =
    let i = 
        try Str.search_forward (Str.regexp_string m) s 0 
        with Not_found -> -1
    in
        i > -1

let string_find_all (s:string) (r:string) = 
    let rec string_find_p (s:string) (re:Str.regexp) (i:int) (acc:string list) =
        try
            let   _ = Str.search_forward re s i in
            let pos = (Str.match_end ()) and
                str = (Str.matched_string s)
            in
            (* let () = Printf.printf "s=%d, p=%d, ?=%s\n" start pos str in *)
            string_find_p s re pos (str::acc)
        with
            Not_found -> acc
    in
    List.rev (string_find_p s (Str.regexp r) 0 [])

let float_of (s: string option) =
    match s with
    | Some n -> (
        try Float.of_string n
        with Failure _ -> 0.0)
    | None -> 0.0

let int64_of (s: string option) =
    match s with
    | Some n -> (
        try Int64.of_string n
        with Failure _ -> 0L)
    | None -> 0L

let value_of (s: string option) (y: string) =
    match s with
    | Some x -> x
    | None -> y

let seq_of_ints (s:int) (e:int) = 
    let rec seq_of_ints_p (s:int) (e:int) (acc: int list) =
        if s < e
            then seq_of_ints_p (s+1) e (s::acc)
            else acc
    in
    seq_of_ints_p s e []

