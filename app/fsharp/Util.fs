module Util 

open System
open System.IO
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop

// Mono.Unix/Syscall.cs
let S_ISUID     = 0x0800u // Set user ID on execution
let S_ISGID     = 0x0400u // Set group ID on execution
let S_ISVTX     = 0x0200u // Save swapped text after use (sticky).
let S_IRUSR     = 0x0100u // Read by owner
let S_IWUSR     = 0x0080u // Write by owner
let S_IXUSR     = 0x0040u // Execute by owner
let S_IRGRP     = 0x0020u // Read by group
let S_IWGRP     = 0x0010u // Write by group
let S_IXGRP     = 0x0008u // Execute by group
let S_IROTH     = 0x0004u // Read by other
let S_IWOTH     = 0x0002u // Write by other
let S_IXOTH     = 0x0001u // Execute by other
let S_IRWXG     = (S_IRGRP ||| S_IWGRP ||| S_IXGRP)
let S_IRWXU     = (S_IRUSR ||| S_IWUSR ||| S_IXUSR)
let S_IRWXO     = (S_IROTH ||| S_IWOTH ||| S_IXOTH)
let ACCESSPERMS = (S_IRWXU ||| S_IRWXG ||| S_IRWXO) // 0777
let ALLPERMS    = (S_ISUID ||| S_ISGID ||| S_ISVTX ||| S_IRWXU ||| S_IRWXG ||| S_IRWXO) // 07777
let DEFFILEMODE = (S_IRUSR ||| S_IWUSR ||| S_IRGRP ||| S_IWGRP ||| S_IROTH ||| S_IWOTH) // 0666

[<DllImport ("libc", SetLastError=true, EntryPoint="mkfifo")>]
extern int _mkfifo (string pathname, uint mode)

let mkfifo (pathname:string) (mode:uint) = 
    _mkfifo (pathname, mode)

let readLine(f:string) = 
    let stream = new StreamReader(f) 
    let line = stream.ReadLine()
    stream.Close()
    line

let isNull (x: _ nativeptr) = 
    x = (NativePtr.ofNativeInt IntPtr.Zero)
    //let j = NativePtr.read x
    //obj.ReferenceEquals (j, null)

let isNotNull (x: _ nativeptr) = not (isNull x)
