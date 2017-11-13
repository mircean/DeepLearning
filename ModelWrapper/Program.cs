using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Python.Runtime;

namespace ModelWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            string pythonPath = @"C:\ProgramData\Anaconda3\Lib\site-packages";
            string pythonCode = @"..\..\..\TestKeras.py";
            string modelFile = @"..\..\..\MNIST\keras.h5";
            string input = @"..\..\..\MNIST\test.csv";
            string output = @"..\..\..\MNIST\predict.csv";

            ModelWrapper model = new ModelWrapper(pythonPath,
                pythonCode,
                modelFile);

            model.Predict(input, output);

            using (StreamReader sr = new StreamReader(output))
            {
                var classId = sr.ReadLine();
                Debug.Assert(classId == "2");
            }
        }
    }

    class ModelWrapper
    {
        string m_pythonCode;
        string m_modelFile;

        public ModelWrapper(string pythonPath,
            string pythonCode,
            string modelFile)
        {
            FileInfo fileInfo = new FileInfo(pythonCode);

            using (Py.GIL())
            {
                dynamic py_sys = Py.Import("sys");
                py_sys.path.append(pythonPath);
                py_sys.path.append(fileInfo.Directory.FullName);
            }

            m_pythonCode = Path.GetFileNameWithoutExtension(fileInfo.Name);
            m_modelFile = modelFile;
        }

        public void Predict(string input, string output)
        {
            using (Py.GIL())
            {
                dynamic py_model = Py.Import(m_pythonCode);
                py_model.predict(m_modelFile, input, output);
            }
        }
    }
}
