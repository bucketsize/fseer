module Util 

open System
open System.IO

let readLine(f:string) = 
    let stream = new StreamReader(f) 
    let line = stream.ReadLine()
    stream.Close()
    line

