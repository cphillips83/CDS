My thoughts of the project going forward


Features
- Support branches
- Should be able to compare manifest across branches to bring up to date
- Commits are based on SHA256 of the manifest
- Clients should default to read-only mode (can't commit)


Commits
- You generate the manifest of your local machine
- You get a change set from the last manifest applied
- You send the change set to the server and it will 
    reply with the files it needs as well as a commit lock GUID
- Once all files are sent to the server, you finalize the commit
    via the lock GUID and the server will reply true/false
- Exist outside of branches and branches only refer to commits             
    
Client
- Store user and current branch
- Keeps a list of all branches it has access to
- Will only know the head SHA of outside branches
- Fetch will retrieve all manifests of branches it has access to
    
Server
- Will run as a stand alone process
- Can create multiple repositories
- Each repository can have multiple branches with a default master
- Should be able to delete old commits
- Should be able to compact to remove files without references
- Determine authorization mode, must be per branch
    Should users have a default branch then?
- 

Client Commands 
- fetch     = Get the latest manifests (including branches)
- update    = Move to the head manifest of current branch
                -dry    dry run (show what will happen)
- init      = Initializes a directory for cds (defaults to read-only)
- checkout <branch>
            = Switches to a different branch and applies the head manifest for that branch
- reset     = Will revert anything status shows as changes (skips anything not in manifest when in read-only)
- status    = Shows all local changes (skips anything not in manifest when in read-only)
- log       = Shows the last 25 manifest logs and their SHA/Decription
                -commit  <SHA>  will instead show the changeset of a specific commit
- commit <message>  (only when not in read-only)
            = Will stage and commit all files from the result of status and advanced the head
- pull		= Combines fetch and pull