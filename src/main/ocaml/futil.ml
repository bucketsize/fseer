
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

let extract_in_between (s:string) (e:string) (line:string) =
    let is = Str.search_forward (Str.regexp s) line 0  in
    let ie = Str.search_forward (Str.regexp e) line is in
    String.sub line (is + String.length s) (ie-is-(String.length s))

let has_in (s:string) (m:string) =
    Str.string_match (Str.regexp m) s 0 
