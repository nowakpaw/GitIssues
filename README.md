Missing elements:
- No tests (should I create them?)
- No login functionality
- No proper AAA (I relied on PAT tokens)

What would I do differently?
- Used Refit and/or non-anonymous request objects for Git services (I assumed Refit might violate task guidelines).
- Made constants more flexible in specific client implementations - there are differences like close/closed (issue state). I considered using appsettings, and if this were part of a larger system (e.g., with a database), I'd store them there (using resx would require full rebuild for changes, while appsettings only needs app restart).

Additionally, I separated Shared (so I have three projects instead of two), but somewhat unnecessarily (I could have kept common contracts/extensions etc. with the client, e.g., in Blazor).

<b>I didn’t create a GitLab account (there’s a free trial), but I believe it should work unless I missed something in their docs. :)</b>

The small number of services would normally push me toward extracting a base class rather than using a pure strategy pattern + helper, but I thought this approach was clearer (I actually reworked it — so the commit history shows an older version).

![image](https://github.com/user-attachments/assets/2d49d585-f7db-4cb5-9f9f-1b4355909c92)
