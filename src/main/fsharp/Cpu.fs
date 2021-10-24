module Cpu 

open System
open System.IO

type CpuInfo = {
    arch: string;
    usage: float32;
}

let cputime cs0 cz0 =
    let stream = new StreamReader @"/proc/stat"
    let line = stream.ReadLine()
    stream.Close()
    let (_::values) = Seq.toList (line.Split [|' '|]) 
    let cputs =
        values 
        |> List.filter (fun x -> (x <>  "")) 
        |> List.map Int32.Parse
    let cs, cz = List.sum cputs, cputs.Item 3 
    cs, cz

let mutable cs, cz = 0, 0

let info ()  = 
    let css, czz = cputime cs cz
    let cu = (1.0f - ((float32 (czz-cz))/(float32 (css-cs))))
    cs <- css  
    cz <- czz 
    {arch = ""; usage = cu}

