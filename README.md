ThinkpadTouchPointTapGesture
====================

ThinkpadTouchPointTapGesture : A gesture recognizer for a tapping gesture on a touchpoint of Thinkpad PC.

Introduction
--------------------

**ThinkpadTouchPointTapGesture** is a gesture recognizer. It recognize tapping gestures on a touchpoint of a Thinkpad note PC.

![Imgur](http://i.imgur.com/Jetu9Cr.png)


Software
--------------------

If this software detects a tapping gesture, it shows a text "Tapped!."

If you check "タップ検出時にマウスクリックする," this software executes a left click automatically when a tapping gesture is detected.

![Imgur](http://i.imgur.com/CWO5oVK.png?1)


Implementation detail
--------------------

This gesture recognizer looks mouse cursor's movement. 
It assumes that the touchpoint is tapped if the mouse cursor moved subtly in a moment.

Because the current implementation is too naive, the gesture recognition accuracy is not good. 


This software is implemented as a WinForm application with Visual C# 2015.
I confirm this software works on ThinkPad X1 Carbon in Windows 10.


Licence
--------------------

The MIT License (MIT)

Copyright (c) 2016 furaga

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.