namespace TestProject1
{
    interface ISomeInterface
            {
                void Firstmethod();

                void Secondmethod();
            }
        
            class Class : ISomeInterface
            {
        
                public void Firstmethod()
                {
                    throw new System.NotImplementedException();
                }
        
                public void Secondmethod()
                {
                    throw new System.NotImplementedException();
                }
            }
        
            interface ITestClass
            {
                void Firstmth();
        
                void Secondmth();
            }
        
            class TestClass : ITestClass
            {
                public ISomeInterface isomeInterface;
        
                public TestClass(ISomeInterface isomeInterface)
                {
                    this.isomeInterface = isomeInterface;
                }
        
                public void Firstmth()
                {
                    throw new System.NotImplementedException();
                }
        
                public void Secondmth()
                {
                    throw new System.NotImplementedException();
                }
            }
        
            class TestClass2 : ITestClass
            {
                public ISomeInterface isomeInterface;
        
                public TestClass2(ISomeInterface isomeInterface)
                {
                    this.isomeInterface = isomeInterface;
                }
        
                public void Firstmth()
                {
                    throw new System.NotImplementedException();
                }
        
                public void Secondmth()
                {
                    throw new System.NotImplementedException();
                }
            }
        
            class Class2 : ISomeInterface
            {
                public void Firstmethod()
                {
                    throw new System.NotImplementedException();
                }
        
                public void Secondmethod()
                {
                    throw new System.NotImplementedException();
                }
            }

            interface IB
            {
            
            }

    interface IC
    {

    }
    class ClassB : IB
            {
                public IA ia { get; set; }
                public IC ic { get; set; }
                public ClassB(IA ia, IC ic)
                {
                    this.ia = ia;
                    this.ic = ic;
                }
             
            }



            class ClassC : IC
            {
                public IB ib { get; set; }
                public ClassC(IB iB)
                {
                    this.ib = ib;
                }

            }

    interface IA
            {
            
            }

            class ClassA : IA
            {
                public IB ib { get; set; }
                public ClassA(IB ib)
                {
                    this.ib = ib;
                }
              
            }
           interface ISelf
           {
             
           }

           class Self : ISelf
           {
               public ISelf iself { get; set; }

               public Self(ISelf self)
               {
                   this.iself = self;
               }
               
           }
           interface IQ
           {
     
           }

           class Q : IQ
           {
               public IW iw { get; set; }
               public Q(IW iw)
               {
                   this.iw = iw;
               }

            
           }

           interface IW
           {
       
           }

           class W : IW
           {
               public IE ie { get; set; }
               public W(IE ie)
               {
                   this.ie = ie;
               }
               
           }

           interface IE
           {
        
           }

           class E : IE
           {
               public IQ iq { get; set; }
               public E(IQ iq)
               {
                   this.iq = iq;
               }
               
           }

           


}