# UniFramework.Event

一个轻量级的事件系统。

## EventGroup的作用

在当前类中new EventGroup() 然后通过group注册事件，可以通过group记录已注册的事件。然后group提供了移除所有事件的方法。从而不必再一一移除事件。
