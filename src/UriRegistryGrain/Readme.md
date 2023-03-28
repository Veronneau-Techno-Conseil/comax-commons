# URI Registry

The UriRegistry grain will serve as a ledger to store any resource URI with its matching GUID.

Other grain who try to push data into stream will rely on UriRegistry grain to determine the stream ID based on GUID held in the UriRegistry grain associated to the target uri.



<a href="https://imgbb.com/"><img src="https://i.ibb.co/YWbxsJ0/uriregistry.png" alt="uriregistry" border="0"></a>
