# Server Sent Events
Dokumentace [Developer Mozilla](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events
)

Pro komunikace mezi klientem a serverem prostřednictvím HTTP protokolu je standardně použivaný proces typu REQUEST/RESPONSE.

Jde tedy o to, že klient zašle požadavek, tím dojde k navázání spojení se serverem. Server následně vykoná svůj kód odešle odpověď a ukončí spojení.

Pro většinu scánářů to stačí. Co ale dělat v případě, že potřebujeme, aby server informoval událostech na své straně?

##### Existují minimálně tyto 4 techniky:
1. vyvolání Ajax requestu na straně klienta (teda v případě, že se jedná o web prohlížeč) - toto řešení je sice korektní, ale má problém hlavně se procesem, kdy je opakovaně navazováno spojení se serverem, což stojí nějaký čas a prostředky
1. použití techniky zvané LongPolling - toto je spíše obsolete řešení, jde o jednosměrnou komunikaci
1. použití WebSocketu - "nejnovější" způsob komunikace mezi klientem a serverem, umožňuje oboustrannou komunikaci
1. EventSource - jednosměrná komunikace, změreme ze serveru na klienta

## EventSource
Princip komunikace je postaven na standardním HTTP protokolu, neměl by tedy vznikat problém na různých proxy či gateway. Typicky CloudFlare ještě do nedávna neumožňoval použití WebSocketů. 

### Zjednodušený popis
1. klient naváže se serverem standardní HTTP spojení, typicky pomocí metody GET
1. server požadavek přijme a odešle zpět hlavičku **text/event-stream**, spojení ale neukončí
1. klient přijme odpověď s hlavičkou, spojení stále zůstává navázáno
1. server průběžně posílá informace do **streamu** na klienta formou "plain textu" s formátováním:
  - id: XX EOL *identifikátor zprávy*
  - event: message EOL *název eventu, podle kterého se může na klientovi vyvolat událost*
  - data: text EOL *samotná zpráva obsahující data posílané ze serveru*
  - retry: number EOL *počet milisekund za jak dlouho se má udělat reconnect na server v případě přerušení*
  - EOL *oddělovač jednotlivých zpráv*

EventSourcing podporuje také požadavky mezi doménamy, tzv. CORS. Za tímto účelem je potřeba na serveru nastavit správně hlavičky CORS.

Samotná třída EventSource ovšem neumí posílat standardní HTTP hlavičky, tudíž se neumí autorozovat běžným způsobem. Toto je možné vyřešit pomocí jednorázového časově omezeného tokenu anebo použitím JS knihovny překrývající funkčnost EventSourcingu a umožňujícího tedy posílat hlavičky, včetně např. autorizace JWT tokenem. Tato knihovna je dostupná na (GitHubu)[https://github.com/Yaffle/EventSource]

