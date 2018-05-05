using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace QuickBuild
{

    public class QBEditorLayoutIndent : IDisposable
    {
        public QBEditorLayoutIndent()
        {
            ++EditorGUI.indentLevel;
        }
        
        #region IDisposable implementation
        public void Dispose()
        {
            --EditorGUI.indentLevel;
        }
        #endregion
        
    }

}