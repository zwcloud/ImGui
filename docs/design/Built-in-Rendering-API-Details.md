# General

1. Redraw every meshes every frame. __No dirty region__ that marks rectangles needs to be repaint like legacy CPU-based rendering system. If any control is *hot*, then repaint it every frame.
2. Optimization:
	* Clip the meshes against the scroll rectangle, the window and the form.
	* Update only hot and active control's primitive: no dirty flag.
	* how to reduce mesh size and number, texture size and number, off-screen buffer size and buffer and other GPU resources.

# Details

## important: Text-related caches

It is too expensive to rebuild text meshes for a piece of text with format. We need to cache it and also keep the flexibility to not rebuild too much when a piece of text and its format is changed.

Then we need a cache system that caches all proper layers of text-related objects and values.

a list of text-related objects of different layers:

1. `GlyphData` => `GlyphCache`
2. `TextContext` and `TextPrimitive` => `TextContextCache` and `TextPrimitiveCache`
3. `TextMesh` => `TextMeshCache`

This should be considered carefully to balance the cache performance and memory occupation.

Here is a list of text properties:

| Property     | TextPrimitive | TextContext | TextMesh |
|--------------|---------------|-------------|----------|
| text         |       Y       |      Y      |    Y     |
| font family  |       Y       |      Y      |    Y     |
| font size    |       Y       |      Y      |    Y     |
| font color   |       N       |      N      |    Y     |
| font style   |       Y       |      Y      |    Y     |
| font weight  |       Y       |      Y      |    Y     |
| alignment    |       Y       |      Y      |    Y     |
| font stretch |      N/A      |     N/A     |   N/A    |

All text properties except font color have influences on `TextPrimitive`, `TextContext` and `TextMesh`. Note font stretch will not be supported since almost all web browsers have decided not to support it.

# Update a Primitive

We should be able to update a primitive without completely recreating it. For example, a rectangle `PathPrimitive`, the size of this rectangle is changed, then only the corrdinates of added line-segments should be changed:

	//create a line-segment
	public void PathLineTo(Point point)
	{
		Path.Add(new LineToCommand(point));
	}

	//modify a line-segment
	//1. find the LineToCommand that should be modified
	var lineToCommand = FindLineCmd(Path, ...);
	//2. modify the end point
	lineToCommand.Point = new Point(newX, newY);

At present, there is no implementation of `FindLineCmd`.