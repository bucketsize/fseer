open Futil
open Fseerrec.Net

let ni = {
    intfs = [
        ("none", {name="none"; rx=0L; tx=0L; dr=0.0; dt=0.0});
    ]}

let pi = Float.of_int Consts.poll_interval

let info zfn =
    let intfs = 
        read_file_lines "/proc/net/dev"
        |> List.filter (fun x -> not (has_in x "Inter"))
        |> List.filter (fun x -> not (has_in x " face" ))
        |> List.map (fun x -> 
            let xs = String.split_on_char ':' x in
            let nd = String.trim (List.nth xs 0) and
                ns = List.nth xs 1 in 
            let nv = 
                string_find_all ns "[0-9]+"
                |> List.map (fun x -> Int64.of_string x)
            in
            let rx = List.nth nv 0 and
                tx = List.nth nv 1 in
            match get_item ni.intfs nd with
            | Some (_, intf) -> 
                    let dr = (Int64.to_float (Int64.sub rx intf.rx)) /. pi and
                        dt = (Int64.to_float (Int64.sub tx intf.tx)) /. pi in
                    (nd, {name = nd;
                            rx = rx;
                            tx = tx;
                            dr = dr;
                            dt = dt})
            | None ->
                    (nd, {name = nd;
                            rx = rx;
                            tx = tx;
                            dr = 0.0; 
                            dt = 0.0})
            )
    in
    let () = ni.intfs <- intfs
    in ni

