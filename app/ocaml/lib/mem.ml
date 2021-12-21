open Futil

type mem_info = {
    mutable usage: float;
    mutable mt: float;
    mutable mf: float;
    mutable st: float;
    mutable sf: float;
}

let info () =
    let mi = {usage=0.0; mt=0.0; mf=0.0; st=0.0; sf=0.0} in
    let mlines = read_file_lines "/proc/meminfo" in
    let parsfn = 
        (fun mi line -> 
            mi.mt <- 
                if has_in line "MemTotal: " 
                then float_of (extract_in_between "MemTotal: " " kB" line)
                else mi.mt;
            mi.mt <-
                if has_in line "MemTotal: " 
                then float_of (extract_in_between "MemTotal: " " kB" line)
                else mi.mt;
            mi.mf <- 
                if has_in line "MemFree: "
                then float_of (extract_in_between "MemFree: " " kB" line)
                else mi.mf;
            mi.st <-
                if has_in line "SwapTotal: "
                then float_of (extract_in_between "SwapTotal: " " kB" line)
                else mi.st;
            mi.sf <- if has_in line "SwapFree: "
                then float_of (extract_in_between "SwapFree: " " kB" line)
                else mi.sf;
            mi) in
    let mi1 = List.fold_left parsfn mi mlines in
    (* let () = Printf.printf "mem: %f, %f, %f, %f" mi1.mf mi1.mt mi1.sf mi1.st in *)
    let () = mi1.usage <- ((mi1.mt -. mi1.mf) /. mi1.mt) in
    mi1

