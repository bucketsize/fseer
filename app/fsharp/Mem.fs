module Mem

open System
open System.IO

type MemInfo = {
    usage: float32;
}

let extract (s:string) (e:string) (line:string) =
    let is, ie = line.IndexOf(s), line.IndexOf(e)
    //printfn "%d, %d, %d" is ie line.Length
    line.Substring(is+s.Length, ie-is-s.Length)

let meminfo ()  =
    let mutable mt, mf, st, sf = 0, 0, 0, 0
    File.ReadLines  @"/proc/meminfo"
    |> Seq.toList
    |> List.fold (fun _ line -> 
        mt <- if mt = 0 && line.IndexOf("MemTotal: ") >= 0 
                then Int32.Parse(extract "MemTotal: " "kB" line)
                else mt

        mf <- if mf = 0 && line.IndexOf("MemFree: ") >= 0
                then Int32.Parse(extract "MemFree: " "kB" line)
                else mf

        st <- if st = 0 && line.IndexOf("SwapTotal: ") >= 0
                then Int32.Parse(extract "SwapTotal: " "kB" line)
                else st
                
        sf <- if sf = 0 && line.IndexOf("SwapFree: ") >= 0
                then Int32.Parse(extract "SwapFree: " "kB" line)
                else sf
        //printfn "%s" line        
        //printfn "%s" ((mt,mf,st,sf).ToString())
        (mt,mf,st,sf)
    ) (mt,mf,st,sf)
    
let info () =
    let mt,mf,st,sf = meminfo()
    {usage = ((float32)(mt-mf)/(float32)mt)}

