open Futil

type mem_info = {
    mutable usage: float;
    mutable mt: int64;
    mutable mf: int64;
    mutable st: int64;
    mutable sf: int64;
}

let info () =
    let mi = {usage=0.0; mt=0L; mf=0L; st=0L; sf=0L} in
    let mlines = read_file_lines "/proc/meminfo" in
    let parsfn = 
        (fun mi line -> 
            mi.mt <-
                if has_in line "MemTotal: " 
                then int64_of (extract_in_between "MemTotal: " " kB" line)
                else mi.mt;
            mi.mf <- 
                if has_in line "MemFree: "
                then int64_of (extract_in_between "MemFree: " " kB" line)
                else mi.mf;
            mi.st <-
                if has_in line "SwapTotal: "
                then int64_of (extract_in_between "SwapTotal: " " kB" line)
                else mi.st;
            mi.sf <- if has_in line "SwapFree: "
                then int64_of (extract_in_between "SwapFree: " " kB" line)
                else mi.sf;
            mi) in
    let mi1 = List.fold_left parsfn mi mlines in
    (* let () = Printf.printf "mem: %Ld, %Ld, %Ld, %Ld\n" mi1.mf mi1.mt mi1.sf mi1.st in *)
    let mf = (Int64.to_float mi1.mf) and
        mt = (Int64.to_float mi1.mt) in
    let () = mi1.usage <- (1.0 -. (mf /. mt)) in
    mi1

