open Fseer.Futil
open OUnit2

let t01_has_in _ =
    assert_equal true (has_in " he who comes; goes." "come");
    assert_equal true (has_in " he who comes; goes." "he");
    assert_equal true (has_in " he who comes; goes." "goes.")

let t02_has_in _ =
    assert_equal false (has_in " he who comes; goes." "that")

let t03_string_find_all _ =
    let t = string_find_all "a b 22 4 6 0if" "[0-9]+" in
    assert_equal ["22"; "4"; "6"; "0"] t 
   
let t04_extract_in_between _ =
    let t = extract_in_between "come" "you" "come as - 22 you go!" in
    assert_equal " as - 22 " (value_of t "?")

let t05_1_extract_in_between _ =
    let t = extract_in_between "come: " "you" "come: as - 22 you go!" in
    assert_equal "as - 22 " (value_of t "?")

let t05_2_extract_in_between _ =
    let t = extract_in_between "come: " " you" "come: as - 22 you go!" in
    assert_equal "as - 22" (value_of t "?")

let t06_float_of _ = 
    assert_equal 22.0 (float_of (Some "22"));
    assert_equal 22.0 (float_of (Some " 22")); 
    assert_equal 22.0 (float_of (Some "     22")) 

let t07_float_of _ = 
    assert_equal 0.0 (float_of (Some "22 "));
    assert_equal 0.0 (float_of (Some " 22 ")); 
    assert_equal 0.0 (float_of (Some "22     "));
    assert_equal 0.0 (float_of (Some "   22  ")); 
    assert_equal 0.0 (float_of (Some "x22"));
    assert_equal 0.0 (float_of (Some "  ss   22")); 
    assert_equal 0.0 (float_of (Some "22  ss   "));
    assert_equal 0.0 (float_of (Some "s   22  s")) 

let suite =
  "FutilTest" >::: [
    "t01_has_in" >:: t01_has_in;
    "t02_has_in" >:: t02_has_in;
    "t03_string_find_all" >:: t03_string_find_all ;
    "t04_extract_in_between" >:: t04_extract_in_between ;
    "t05_1_extract_in_between" >:: t05_1_extract_in_between ;
    "t05_2_extract_in_between" >:: t05_2_extract_in_between ;
    "t06_float_of" >:: t06_float_of ;
    "t07_float_of" >:: t07_float_of ;
  ]

let () =
  run_test_tt_main suite
