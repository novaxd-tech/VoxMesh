using UnityEditor;
using System.IO;
using UnityEditor.Build;
using UnityEngine;

class PostBuildProcessor : IPostprocessBuild
{
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildTarget target, string path)
    {
        // Copia um arquivo da pasta do projeto para a pasta da build, junto com o jogo construído.
        Debug.Log("pop");
        FileUtil.CopyFileOrDirectory("server.py", path + "server.py");
    }
}
