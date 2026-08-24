namespace PadelManager.Interfaces;

// Levée par IMatchRepository.InscrireEtPayerAsync lorsque le membre a déjà une participation sur
// ce match (filet de sécurité redondant avec UQ_PARTICIPATION_match_membre).
public class DejaInscritException : Exception {
}
