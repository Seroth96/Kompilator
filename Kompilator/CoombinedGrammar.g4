grammar CoombinedGrammar; 

prog: ( stat? NEWLINE )* 
    ;

stat:   PRINT value		#print
       |    ID '=' value       #assign
	   | READ ID	#read
   ;

value: ID
       | STRING	
	   | INT	
	   | expr0
	   | vector
   ;

vector: '[' INT (',' INT)* ']'
   | '[' ']'
   ;

expr0:  expr1			#single0
      | expr1 ADD expr1		#add 
;

expr1:  expr2			#single1
      | expr2 MULT expr2	#mult 
;

expr2:   INT			#int
       | REAL			#real
       | TOINT expr2		#toint
       | TOREAL expr2		#toreal
       | '(' expr0 ')'		#par
;	

TOINT: '(int)'
    ;

TOREAL: '(real)'
    ;

PRINT:	'print' 
   ;
   
READ:	'read'
	;

STRING :  '"' ( ~('\\'|'"') )* '"'
    ;

ID:   ('a'..'z'|'A'..'Z')+ INDEX?
   ;

INDEX: '_' INT '_'
	   ;

REAL: '0'..'9'+'.''0'..'9'+
    ;

INT:   '0'..'9'+
    ;

ADD: '+'
    ;

MULT: '*'
    ;

NEWLINE:	'\r'? '\n'
    ;

WS:   (' '|'\t')+ -> skip
    ;
