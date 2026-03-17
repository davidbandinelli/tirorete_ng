# PBEM Football - Guida al Sistema Interattivo

## Avvio del Sistema

Quando avvii il server, ti verrà chiesto se vuoi usare il menu interattivo:
```
Avviare il menu interattivo? (s/n, default s):
```

Premi INVIO o digita 's' per avviare il menu interattivo.

## Menu Principale

```
╔════════════════════════════════════════════════════════════════════════╗
║                    PBEM FOOTBALL - MENU PRINCIPALE                     ║
╠════════════════════════════════════════════════════════════════════════╣
║ 1. Inserisci dati squadre partecipanti                                 ║
║ 2. Visualizza tutte le squadre                                         ║
║ 3. Crea nuova stagione                                                 ║
║ 4. Visualizza classifica campionato                                    ║
║ 5. Visualizza classifica marcatori                                     ║
║ 6. Simula una giornata                                                 ║
║ 0. Esci                                                                ║
╚════════════════════════════════════════════════════════════════════════╝
```

## Funzionalità

### 1. Inserisci dati squadre partecipanti

Permette di inserire i dati delle squadre che parteciperanno al campionato.

**Dati richiesti per ogni squadra:**
- **Nome squadra**: Il nome della squadra
- **Nome presidente/manager**: Il nome del presidente o manager
- **PA (Punti Allenamento)**: Punti da usare per l'allenamento (default: 5)
- **PGP (Punti Grande Prestazione)**: Punti per grandi prestazioni (default: 30)
- **M (Milioni)**: Budget in milioni (default: 0)

**Numero squadre:** Di default 12, ma puoi specificare un numero diverso.

**Esempio:**
```
Numero di squadre (default 12): 4

--- Squadra 1 ---
Nome squadra: FC Beppiland
Nome presidente/manager: Mario Rossi
PA (Punti Allenamento, default 5): 7
PGP (Punti Grande Prestazione, default 30): 35
M (Milioni, default 0): 10
```

### 2. Visualizza tutte le squadre

Mostra una tabella con tutte le squadre inserite e i loro dati:

```
╔════════════════════════════════════════════════════════════════════════╗
║                       SQUADRE PARTECIPANTI                             ║
╠════════════════════════════════════════════════════════════════════════╣
║ N.  Squadra              Manager              PA  PGP   M              ║
╠════════════════════════════════════════════════════════════════════════╣
║  1. FC Beppiland         Mario Rossi           7   35   10             ║
║  2. AC Juniores          Luigi Verdi           5   30    5             ║
...
```

### 3. Crea nuova stagione

Crea una nuova stagione di campionato con tutte le squadre inserite.

**Cosa succede:**
- Genera il calendario completo (andata e ritorno)
- Crea la Coppa Beppiland (eliminazione diretta)
- Distribuisce le partite in 11 sessioni
- Inizializza le classifiche

**Requisiti:** Servono almeno 3 squadre inserite.

**Esempio output:**
```
✓ Stagione 2024 creata con successo!
  - Squadre in Serie A: 12
  - Competizioni create: 4
  - Sessioni di gioco: 11
  - Partite totali: 264
```

### 4. Visualizza classifica campionato

Mostra la classifica aggiornata del campionato:

```
╔════════════════════════════════════════════════════════════════════════╗
║                      CLASSIFICA CAMPIONATO                             ║
╠════════════════════════════════════════════════════════════════════════╣
║ Pos  Squadra                G   V  P  S   GF  GS  DR  Punti           ║
╠════════════════════════════════════════════════════════════════════════╣
║  1.  FC Beppiland           5   4  1  0    12   3   9   13            ║
║  2.  AC Juniores            5   3  2  0     8   4   4   11            ║
...
```

**Legenda:**
- **G**: Partite Giocate
- **V**: Partite Vinte
- **P**: Partite Pareggiate
- **S**: Partite Sconfitte (Lost)
- **GF**: Goal Fatti
- **GS**: Goal Subiti
- **DR**: Differenza Reti

### 5. Visualizza classifica marcatori

Mostra i migliori marcatori del campionato:

```
╔════════════════════════════════════════════════════════════════════════╗
║                      CLASSIFICA MARCATORI                              ║
╠════════════════════════════════════════════════════════════════════════╣
║ Pos  Giocatore           Squadra              Goal                     ║
╠════════════════════════════════════════════════════════════════════════╣
║  1.  Andrea Neri         FC Beppiland            8                     ║
║  2.  Francesco Arancio   AC Juniores            6                     ║
...
```

**Include:**
- Goal da azione
- Rigori segnati
- Ordine per numero di goal (decrescente)

### 6. Simula una giornata

Simula tutte le partite della prossima giornata di campionato non ancora giocata.

**Esempio output:**
```
Giornata 3:

FC Beppiland         2-1 AC Juniores        
Serie A Team 1       0-0 Serie A Team 2     
Serie A Team 3       3-2 Serie A Team 4     
...

✓ Giornata 3 completata!
```

**Dopo ogni giornata:**
- Le partite vengono simulate automaticamente
- La classifica viene aggiornata
- I goal vengono registrati per la classifica marcatori

## Flusso di lavoro tipico

1. **Avvia** il server
2. **Inserisci** i dati delle squadre (opzione 1)
3. **Visualizza** le squadre per verificare (opzione 2)
4. **Crea** una nuova stagione (opzione 3)
5. **Simula** le giornate una alla volta (opzione 6)
6. **Visualizza** la classifica dopo ogni giornata (opzione 4)
7. **Visualizza** la classifica marcatori (opzione 5)

## Note Tecniche

- Le formazioni per le simulazioni sono generate automaticamente (4-3-3)
- Ogni squadra parte con una rosa completa di 22 giocatori
- Il sistema supporta tutte le regole del gioco PBEM Football
- I rigori sono conteggiati nella classifica marcatori

## Prossimi Sviluppi

- Configurazione formazioni personalizzate
- Gestione tattiche per ogni squadra
- Coppa di Lega e Coppa Juniores
- Mercato trasferimenti
- Sistema di allenamento
- Salvataggio/caricamento stagioni
