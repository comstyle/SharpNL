#SharpNL (WiP)

## What is this?

> An awesome and independent reimplementation of the [Apache OpenNLP] software library in C#

## Main differences to the original OpenNLP

> - Pure C#, there is no need to run a virtual machine :stuck_out_tongue_winking_eye:
> - Was built from scratch by hand, without any assist tool in order to maximize the synergy with .net technology. 
> - There are [analyzers](https://github.com/knuppe/SharpNL/wiki/Analyzers) that help a lot the implementation and abstraction of this library.  
> - The heavy operations (like training) can be monitored and cancelled.
> - Some file formats were revamped/improved (Ad and Penn Treebank).


## Goals

> Implement the best library of natural language processing in C#, which means:
> - Be as lightweight as possible
> - Be fully compatible with the OpenNLP library (done!)


## TODO

> - Port the OpenNLP Library ~99% done (The alpha version is almost ready).
> - Improve usability of the library (Good progress so far)
> - Run a Profiler to reduce memory and CPU utilization.
> - Ensure that the library is compatible with Mono.

> [How to contribute](contributing.md)

## Support

As a human being I like to be honest, I believe that someday our kind will transcend money... 
But unfortunately, while we have not reached this day I need some money to make my living. 
If you like this project or need to use this library in the future please consider making a 
donation (anything helps), writing this library takes a HUGE amount of my time, effort and 
resources. But I do it because I'm passionate about it, and hope to make an impact on the 
world while also sharing my little knowledge as a human being.

Please, consider donating as a thank you.

[![donate](resources/donate.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7SWNPAPJNSARC)

[![bean](resources/bean.gif)](#)

[Apache OpenNLP]: http://opennlp.apache.org
[CoGrOO]: http://cogroo.sourceforge.net/
