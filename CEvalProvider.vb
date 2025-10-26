
Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports System.CodeDom.Compiler
Imports System.Reflection
Imports System.IO

Public Class CEvalProvider
    Public Structure MethodObjectAndInfo
        Dim methodObject As Object

    End Structure

    Friend Shared Function GetFunction(ByVal vbCode As String, Optional ByVal params() As CGsfCoder.SParamElem = Nothing) As System.Reflection.MethodInfo
        Dim c As VBCodeProvider = New VBCodeProvider
        Dim icc As ICodeCompiler = c.CreateCompiler()
        Dim cp As CompilerParameters = New CompilerParameters

        cp.ReferencedAssemblies.Add("system.dll")
        cp.ReferencedAssemblies.Add("system.xml.dll")
        cp.ReferencedAssemblies.Add("system.data.dll")
        cp.ReferencedAssemblies.Add("rylCoder.exe")
        ' Sample code for adding your own referenced assemblies

        cp.CompilerOptions = "/t:library"
        cp.GenerateInMemory = True
        Dim sb As StringBuilder = New StringBuilder("")
        sb.Append("Imports System" & vbCrLf)
        sb.Append("Imports System.Xml" & vbCrLf)
        sb.Append("Imports System.Data" & vbCrLf)
        sb.Append("Imports System.Data.SqlClient" & vbCrLf)
        sb.Append("Imports rylCoder" & vbCrLf)

        sb.Append("Class PABLib " & vbCrLf)

        sb.Append("public shared function EvalCode(ByVal params() as CGsfCoder.SParamElem) as Object " & vbCrLf)
        'sb.Append("YourNamespace.YourBaseClass thisObject = New YourNamespace.YourBaseClass()")
        sb.Append(vbCode & vbCrLf)
        sb.Append("End Function " & vbCrLf)
        sb.Append("End Class " & vbCrLf)


        Dim cr As CompilerResults = icc.CompileAssemblyFromSource(cp, sb.ToString())
        For Each k As String In cr.Output
            Debug.WriteLine(k)
        Next

        Dim a As System.Reflection.Assembly = cr.CompiledAssembly
        Dim o As Object
        o = a.CreateInstance("PABLib")
        Dim t As Type = o.GetType()
        Return t.GetMethod("EvalCode")
    End Function
End Class