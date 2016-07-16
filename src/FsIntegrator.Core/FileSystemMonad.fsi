﻿namespace FsIntegrator.FileHandling

open FSharp.Core
open FsIntegrator.Core.General

[<Sealed>]
type FSScript
type FSScript
    with
        static member internal Empty : FSScript
        static member internal Run : FSScript -> unit


type FSBuilder =
    class
        member Yield : 'a -> FSScript

        /// Move file to target
        [<CustomOperation("move", MaintainsVariableSpace = true)>]
        member Move : fs : FSScript * source : string * target : string -> FSScript

        /// Rename file to target filename
        [<CustomOperation("rename", MaintainsVariableSpace = true)>]
        member Rename : fs : FSScript * source : string * target : string -> FSScript

        /// Copy file to target
        [<CustomOperation("copy", MaintainsVariableSpace = true)>]
        member Copy : fs : FSScript * source : string * target : string -> FSScript

        /// Delete current file
        [<CustomOperation("delete", MaintainsVariableSpace = true)>]
        member Delete : fs : FSScript * target : string -> FSScript

        /// Create a directory
        [<CustomOperation("createdir", MaintainsVariableSpace = true)>]
        member MakeDir : fs : FSScript * target : string -> FSScript

        /// Delete a directory
        [<CustomOperation("deletedir", MaintainsVariableSpace = true)>]
        member RemoveDir : fs : FSScript * target : string -> FSScript

        /// Move a directory
        [<CustomOperation("movedir", MaintainsVariableSpace = true)>]
        member MoveDir : fs : FSScript * source : string * target : string -> FSScript
    end

module FileSystem =  
    val fs : FSBuilder

    /// Empty File Script (do nothing)
    val NoFileScript<'a> : 'a -> FSScript