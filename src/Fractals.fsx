open System
open System.Drawing
open System.Windows.Forms

// ==========================================================================
//   boilerplate
// ==========================================================================

// Create a form to display the graphics
let width, height = 500, 500         
let form = new Form(Width = width, Height = height)
let box = new PictureBox(BackColor = Color.White, Dock = DockStyle.Fill)
let image = new Bitmap(width, height)
let graphics = Graphics.FromImage(image)
//The following line produces higher quality images, 
//at the expense of speed. Uncomment it if you want
//more beautiful images, even if it's slower.
//Thanks to https://twitter.com/AlexKozhemiakin for the tip!
graphics.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.HighQuality
let brush = new SolidBrush(Color.FromArgb(0, 0, 0))

box.Image <- image
form.Controls.Add(box) 


// ==========================================================================
//
// AST parts: what constitutes a figure in terms of drawing primitives
//

// generic geometry
type Coordinate = Coord of float * float

type Line = Line of Coordinate * float * float
          | Segment of Coordinate * Coordinate

// visualization
type VisualElement = 
       VLine of Line * float
     | VSet of VisualElement list
     | VEmpty

let inline (--/) p (angle, length) = Line (p, angle, length)

let inline (.=.) l w = VLine (l, w)

// helper
let pos x y = Coord (x,y)

// map a coordinate to a PointF instance
let pf p = 
    let flip x = (float32)height - x
    let (x,y) = match p with Coord(x,y) -> (x,y)
    new PointF((single)x, (single)y |> flip)

let startpoint l = 
  match l with 
  | Line(sp,_,_)  -> sp
  | Segment(sp,_) -> sp

let endpoint l = 
  match l with 
  | Line (Coord(x,y), angle, length) -> pos (x + length * cos angle) 
                                            (y + length * sin angle
  | Segment (_,ep) -> ep

// ==========================================================================
//
// visual components
//

let pi = Math.PI

let rec render (target : Graphics) (brush : Brush)
               (ve : VisualElement) =
    match ve with
    | VLine (l, width) -> let origin = startpoint l |> pf
                          let dest = endpoint l |> pf
                          let pen = new Pen(brush, (single)width)
                          target.DrawLine(pen, origin, dest)
    | VEmpty -> ()
    | VSet vs -> List.iter (render target brush) vs

// ==========================================================================
// 
// DSL components
//

let makeline p angle length =
    (p --/ (angle, length))


let rec assembletree p angle length width =
    if length > 5. && width > 0.1 then
        let l = makeline p angle length
        let p_end = endpoint l
        let ls1 = assembletree p_end (angle - (pi * 0.05)) (length * 0.75) (width * 0.75)
        let ls2 = assembletree p_end (angle + (pi * 0.17)) (length * 0.75) (width * 0.75)
        VSet [l .=. width; ls1; ls2]
    else
        VEmpty

let notEmpty v =
  match v with
  | VEmpty -> false
  | _ -> true

let rec cleantree t =
    match t with
    | VLine _ -> t
    | VEmpty -> t
    | VSet s -> s |> List.map cleantree |> List.filter notEmpty |> VSet

// ==========================================================================
//
// main stuff
//

let t = assembletree (pos 250. 50.) (pi*0.5) 20. 2.

printfn "%A" t

render graphics brush (t |> cleantree)

form.ShowDialog()

(* To do a nice fractal tree, using recursion is
probably a good idea. The following link might
come in handy if you have never used recursion in F#:
http://en.wikibooks.org/wiki/F_Sharp_Programming/Recursion
*)