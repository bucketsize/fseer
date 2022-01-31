Dune project structure and automatic module layouting
=====================================================

Dune stanze `library` allows to mark set of modules as submodule
under a folder, with the `name` attribute.
As long as the library name is included in other folder with `dune`
files, the submodules can be accessed with `Library_name.` suffixed
to the modules.

ex:
/root
    /lib
        /foo
            dune 
            ... (name: foo)
            yo.ml
        /bar
            dune
            ... (name: bar)
            man.ml
        dune 
        ... (library foo bar)
        car.ml
        ... Foo.Yo.do_yo ()
        ... Foo.Man.get_rain ()
