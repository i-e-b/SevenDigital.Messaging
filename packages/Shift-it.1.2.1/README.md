Shift-it
========

Collection of low-level HTTP and FTP file transfer tools

FTP
---
An old and battle hardend FTP client library, works with many
old and cranky FTP servers that the standard .Net libraries can't handle.

Todo:
* Integrate into the rest of the system.

HTTP
----
A HTTP client that can accept a reasonable level of invalid protocol from servers.
This is a blocking, synchronous library that does very little of the work for you.
Uses only .Net sockets, not WebClient or HttpWebRequest, so works around bugs in some
versions of Mono.

Todo:
* Digest authentication
* chunked upload & download
* Maybe: automatically close socket after read-to-end or read-to-timeout
