namespace ViewModels
//Credit to teichgraf (http://kodierer.blogspot.co.uk/) for the extension to WriteableBitMap!
//You need WriteableBitMapEx.Wpf.dll available from https://github.com/teichgraf/WriteableBitmapEx/
//
//This started out as my trying to work out how to do one thing (6 lines of symmetry actually) but getting distracted along the way.

open System
open FsXaml
open System.Windows.Media.Imaging
open Microsoft.FSharp.Core
open System.Windows.Media

type MainView = XAML<"MainWindow.xaml", true>

type MainViewModel()  = 
    inherit WindowViewController<MainView>()  
        [<DefaultValue>] val mutable Wmp: WriteableBitmap
        
        override this.OnLoaded view =
                           this.drawStuff(view) 

        member this.drawStuff(view : MainView) =
                            let centerX = int (view.ViewPortContainer.ActualWidth / 2.)
                            let centerY = int (view.ViewPortContainer.ActualHeight / 2.)
                            let floatCX = float centerX
                            let floatCY = float centerY
                            
                            let x d = int(floatCX  * Math.Cos (d)  ) 
                            let y d = int(floatCY  * Math.Sin (d)  ) 
                            //I had so many other plans for these 4 tuples but they lend themselves to this
                            let points = [for i in 0. .. 360. do 
                                                         yield (  x(i) ,
                                                                  y(i) , 
                                                                   centerX+(x(i)),
                                                                   centerY-(y(i)) )  
                                                                  ] 
                         
                            this.Wmp <- new WriteableBitmap(int view.ViewPortContainer.ActualWidth, int view.ViewPortContainer.ActualHeight,96., 96., PixelFormats.Bgr32, null) 
                            //Note how you actually get the animation effect here, you hook the CompositionTarget.Rendering event with a handler of your own.
                            CompositionTarget.Rendering.Add(fun (e: EventArgs) -> 
                                            this.Wmp.Clear()
                                            for i in points do
                                                    //decompose 4-tuple
                                                    let x1,y1,x2,y2 = i
                                                    //I know there must be a nicer way to get constantly random colours but that can wait for the next commit or two
                                                    let r,g,b = new System.Random() |>fun x-> byte (x.Next(1,255)), byte (x.Next(1,255) / x.Next(1,3)), byte(x.Next(1,255) /  x.Next(1,5))
                                                    let rz = Color.FromRgb(r,g,b)

                                                    this.Wmp.DrawEllipseCentered(x1, y1, 10,10,Colors.Red)
                                                    this.Wmp.DrawEllipseCentered(x2, y2, 10,10,Colors.Green)
                                                    this.Wmp.DrawLineAa(x1,y1,x2,y2,rz))
                            view.ImageViewport.Source <- this.Wmp
                
                     
        
                    

