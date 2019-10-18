using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessObjects
{
    /// Author: Erik Sanne
    /// Desc: Properties and functions of object type ModuleList.
    ///       Maintains a List of all students and a set of functions to interact with the list.
    /// Last modified by Erik Sanne on 24/10/2017
    public class ModuleList
    {
        private List<Student> _list = new List<Student>();

        public void add(Student newStudent)
        {
            _list.Add(newStudent);
        }

        public Student find(int matric)
        {
            foreach (Student p in _list)
            {
                if (matric == p.Matric)
                {
                    return p;
                }
            }

            return null;

        }

        public void delete(int matric)
        {
            Student p = this.find(matric);
            if (p != null)
            {
                _list.Remove(p);
            }

        }

        public List<int> matrics
        {
            get
            {
               List<int> res = new List<int>();
               foreach(Student p in _list)
                   res.Add(p.Matric);
                return res;
            }
           
        }
    }
}
