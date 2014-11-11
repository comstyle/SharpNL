#SharpNL (WiP)

## What is this?

> An awesome and independent reimplementation of the [Apache OpenNLP] software library in C#

## Request

> I need someone else to help test what we have so far, because a mistake can go unnoticed, and honestly I do not like bugs in my code :)

> [How to contribute](contributing.md)

## Main differences to the original component

> Pure C#, there is no need to run a virtual machine :stuck_out_tongue_winking_eye:
> The heavy operations (like training) can be monitored and canceled.
> Some file formats were revamped/improved (Ad and Penn Treebank).
> The command line tool will not be ported (I'm working in a GUI/IDE to the library)

## Warning

> At the moment, there is no published version, the port seems to be going well but I don't have any ETA for the first release. 

So far the project has ~24,210 lines of code!

## Goals

> Implement the best library of natural language processing in C#, which means:
> - Be as lightweight as possible
> - Be fully compatible with the OpenNLP library (done!)


## TODO

> - Port the OpenNLP Library ~99% done (The alpha version is almost ready).
> - Improve usability of the library (Good progress so far)
> - Run a Profiler to reduce memory and cpu utilization.
> - Create a GUI/IDE (WiP - No ETA for the GUI)
> - Ensure that the library is compatible with Mono.
> - I might implement the Bikel’s parser in the future.

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
