# Analyse des Gaps et Spécifications des Écrans MVP

## Gap Analysis - Fonctionnalités Manquantes

### Gaps Identifiés (par ordre de priorité)

| Gap | Status Actuel | Impact Utilisateur | Priorité |
|-----|---------------|-------------------|-----------|
| Système d'authentification | Absent | Critique - Accès sécurisé | 🔴 CRITIQUE |
| Gestion des véhicules | Absent | Critique - Cœur métier | 🔴 CRITIQUE |
| Gestion des conducteurs | Absent | Critique - Ressources humaines | 🔴 CRITIQUE |
| Système d'alertes | Absent | Important - Prévention | 🟡 IMPORTANT |
| Suivi des sinistres | Absent | Important - Conformité | 🟡 IMPORTANT |
| Rapports et tableaux de bord | Absent | Moyen - Analyse | 🟢 SOUHAITÉ |
| Interface mobile | Absent | Moyen - Accessibilité | 🟢 SOUHAITÉ |

## Spécifications Détaillées des Écrans MVP

### 1. AUTHENTIFICATION ET SÉCURITÉ

#### 1.1 Écran de Connexion
**Priorité :** 🔴 CRITIQUE

**Fonctionnalités :**
- Formulaire de connexion (email/mot de passe)
- Option "Se souvenir de moi"
- Lien "Mot de passe oublié"
- Validation côté client et serveur
- Protection contre les attaques par force brute

**Interface :**
```
[LOGO FleetSyncManager]

┌─────────────────────────────────┐
│  Email    [________________]    │
│  Password [________________]    │
│           [ ] Se souvenir       │
│  [        CONNEXION        ]    │
│  Mot de passe oublié ?          │
└─────────────────────────────────┘
```

#### 1.2 Gestion des Rôles
**Rôles définis :**
- **ADMIN :** Accès complet, gestion utilisateurs
- **GESTIONNAIRE :** Gestion véhicules/conducteurs, rapports
- **CONDUCTEUR :** Consultation véhicule affecté, mise à jour km

### 2. TABLEAU DE BORD PRINCIPAL

#### 2.1 Dashboard Administrateur/Gestionnaire
**Priorité :** 🔴 CRITIQUE

**Widgets inclus :**
- Statistiques générales (nb véhicules, conducteurs, sinistres actifs)
- Alertes urgentes (échéances < 30 jours)
- Graphiques de performance (coûts mensuels, km parcourus)
- Véhicules en maintenance
- Derniers sinistres déclarés

**Layout responsive :**
```
┌─────────────┬─────────────┬─────────────┐
│  Véhicules  │ Conducteurs │  Sinistres  │
│     120     │     85      │     3       │
└─────────────┴─────────────┴─────────────┘
┌─────────────────────────┬─────────────────┐
│    Alertes Urgentes     │   Maintenance   │
│ • CT expire dans 15j    │ • 5 véhicules   │
│ • Assurance expire 20j  │ • Planifiés     │
└─────────────────────────┴─────────────────┘
```

#### 2.2 Dashboard Conducteur
**Vue simplifiée :**
- Véhicule(s) affecté(s)
- Prochaines échéances du véhicule
- Formulaire mise à jour kilométrage
- Déclaration rapide de sinistre

### 3. GESTION DES VÉHICULES

#### 3.1 Liste des Véhicules
**Priorité :** 🔴 CRITIQUE

**Fonctionnalités :**
- Tableau avec pagination (50 véhicules/page)
- Filtres : Statut, Marque, Conducteur affecté, Type
- Recherche globale (immatriculation, modèle)
- Tri sur toutes les colonnes
- Actions en lot (export, modification de statut)

**Colonnes affichées :**
| Immatriculation | Marque/Modèle | Conducteur | Statut | Prochaine échéance | Actions |
|----------------|---------------|------------|--------|-------------------|---------|
| AB-123-CD | Peugeot 308 | Dupont J. | Actif | CT - 15j | 👁️ ✏️ 📋 |

#### 3.2 Fiche Véhicule Détaillée
**Priorité :** 🔴 CRITIQUE

**Onglet 1 - Informations Générales**
```
┌─────────────────────────────────────────────┐
│ [Photo Véhicule]  │ Marque: Peugeot         │
│                   │ Modèle: 308 SW          │
│                   │ Immat: AB-123-CD        │
│                   │ Année: 2020             │
│                   │ Kilométrage: 85,420 km  │
│                   │ Carburant: Diesel       │
│                   │ Statut: Actif           │
└─────────────────────────────────────────────┘

Dates Importantes:
• Mise en service: 15/03/2020
• Fin de garantie: 15/03/2023
• Prochain CT: 15/09/2025
• Fin assurance: 31/12/2025
```

**Onglet 2 - Documents**
- Upload/Download des documents (PDF, images)
- Catégories : Carte grise, Assurance, Contrats, Factures
- Historique des versions
- Alertes d'expiration automatiques

**Onglet 3 - Affectations**
- Historique complet des affectations
- Dates de début/fin
- Motif de changement
- Conducteur actuel avec contact

**Onglet 4 - Maintenance**
- Historique chronologique des interventions
- Planification des entretiens futurs
- Coûts et fournisseurs
- Garanties actives

#### 3.3 Formulaire Création/Modification Véhicule
**Étapes du formulaire :**
1. **Identification :** Immatriculation, VIN, marque/modèle
2. **Caractéristiques :** Année, puissance, carburant, couleur
3. **Acquisition :** Date, prix, fournisseur, mode (achat/location)
4. **Assurance :** Compagnie, n° contrat, garanties, échéances
5. **Configuration alertes :** Seuils kilométriques, fréquences entretien

### 4. GESTION DES CONDUCTEURS

#### 4.1 Liste des Conducteurs
**Priorité :** 🔴 CRITIQUE

**Fonctionnalités similaires aux véhicules :**
- Tableau avec filtres (Statut permis, Véhicule affecté)
- Recherche par nom/prénom
- Export des données conducteurs

**Colonnes :**
| Photo | Nom Prénom | Véhicule | Permis | Prochaine visite | Actions |
|-------|------------|----------|--------|------------------|---------|
| 👤 | Dupont Jean | AB-123-CD | B | Visite médicale - 30j | 👁️ ✏️ |

#### 4.2 Profil Conducteur Détaillé
**Priorité :** 🔴 CRITIQUE

**Onglet 1 - Informations Personnelles**
```
┌─────────────────────────────────────────────┐
│ [Photo ID]        │ Nom: DUPONT             │
│                   │ Prénom: Jean            │
│                   │ Date naiss.: 15/06/1980 │
│                   │ Tél: 06.12.34.56.78     │
│                   │ Email: j.dupont@mail.fr │
│                   │ Adresse: 123 rue...     │
└─────────────────────────────────────────────┘

Contact d'urgence:
• Nom: Marie DUPONT
• Relation: Épouse  
• Téléphone: 06.98.76.54.32
```

**Onglet 2 - Permis et Habilitations**
- Types de permis (B, C, D) avec dates d'obtention
- Restrictions et mentions spéciales
- Habilitations spécifiques (transport marchandises dangereuses)
- Dates d'expiration et renouvellements

**Onglet 3 - Véhicules et Affectations**
- Véhicule actuellement affecté
- Historique des affectations avec dates
- Préférences de véhicules

**Onglet 4 - Infractions et Sinistres**
- Liste des contraventions avec statut
- Participation aux sinistres
- Points sur le permis

### 5. GESTION DE LA MAINTENANCE

#### 5.1 Calendrier des Maintenances
**Priorité :** 🟡 IMPORTANT

**Vue calendrier mensuelle :**
```
     Septembre 2025
Lun  Mar  Mer  Jeu  Ven  Sam  Dim
 1    2    3    4    5    6    7
 8    9   [CT]  11   12   13   14
15  [REV] 17   18   19   20   21
22   23   24   25   26   27   28
```

**Légende :**
- 🔴 Urgent (< 7 jours)
- 🟡 Programmé (< 30 jours)  
- 🟢 Planifié (> 30 jours)

#### 5.2 Historique et Suivi
- Liste chronologique par véhicule
- Filtres par type (entretien, réparation, contrôle)
- Coûts et statistiques
- Fournisseurs et évaluations

### 6. GESTION DES SINISTRES

#### 6.1 Déclaration Rapide de Sinistre
**Priorité :** 🟡 IMPORTANT

**Formulaire en étapes :**
1. **Identification :** Véhicule, conducteur, date/heure
2. **Circonstances :** Lieu, météo, description des faits
3. **Dégâts :** Photos, description, estimation
4. **Tiers :** Informations autres véhicules impliqués
5. **Témoins :** Coordonnées si applicables

#### 6.2 Suivi des Dossiers
- Dashboard des sinistres en cours
- Workflow : Déclaré → En cours → Expert → Réparation → Clos
- Communications avec assurances
- Coûts et franchise

### 7. RAPPORTS ET ANALYSES

#### 7.1 Rapports Prédéfinis
**Priorité :** 🟢 SOUHAITÉ

**Rapports disponibles :**
- Coûts par véhicule (mensuel/annuel)
- Kilométrages par conducteur
- Échéancier des renouvellements
- Bilan sinistralité
- Performance maintenance

#### 7.2 Export et Planification
- Formats : Excel, PDF, CSV
- Envoi automatique par email
- Personnalisation des colonnes

### 8. ADMINISTRATION

#### 8.1 Gestion des Utilisateurs
**Priorité :** 🔴 CRITIQUE (Admin uniquement)

**Fonctionnalités :**
- Création/modification comptes utilisateurs
- Attribution des rôles et permissions
- Activation/désactivation comptes
- Logs de connexion et actions

#### 8.2 Paramètres Système
- Configuration des alertes (seuils, fréquences)
- Templates de notifications (email)
- Paramètres de sécurité
- Sauvegarde et maintenance

## Architecture Navigation

### Menu Principal
```
🏠 Tableau de bord
🚗 Véhicules
👥 Conducteurs  
🔧 Maintenance
⚠️ Sinistres
📊 Rapports
⚙️ Administration (Admin)
👤 Profil
🚪 Déconnexion
```

### Design Responsive
- **Desktop :** Menu latéral fixe + zone de contenu
- **Tablette :** Menu latéral escamotable
- **Mobile :** Menu hamburger + navigation simplifiée

Cette spécification détaille l'ensemble des écrans et fonctionnalités du MVP, prêts pour l'implémentation technique.
