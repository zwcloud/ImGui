Unit Test for ImGui
===================

Unit Test is used to gurantee all functions of ImGui work as expected without any issue.

## Standard
* Follows the [Arrange, Act, Assert](http://wiki.c2.com/?ArrangeActAssert) pattern.
* Organized by [the Structuring Unit Tests](http://haacked.com/archive/2012/01/02/structuring-unit-tests.aspx/) approach.
* Easy to be integrated with continuous integration to automatically check for issues when the code is changed.
* Expected rendering result is represented as an expected image for each test; an expected image is generated and verified by human eyes and then used to verify the result of rendering.

# Dependency

(All dependencies are to be removed.)

* Windows Photo Viewer: A picture viewer to show the output picture of some test. (windows-only)
* [CairoSharp](https://github.com/zwcloud/CairoSharp): A software vector rendering lib to programmatically draw the output of some test.
* [Open 3D Model Viewer](https://github.com/acgessler/open3mod): An 3D model viewer to display output 3D model of some test. (windows-only)
