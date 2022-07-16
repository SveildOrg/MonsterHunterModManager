﻿using MediatR;
using MonsterHunterModManager.Application.Common.Interfaces;

namespace MonsterHunterModManager.Application.Features.ModSettings.Commands.DisableMods;

public class DisableModsCommandHandler : IRequestHandler<DisableModsCommand, List<Domain.Entities.ModSettings>>
{
    private readonly IApplicationPersistenceContext _applicationPersistenceContext;
    private readonly IPhysicalFileService _physicalFileService;

    public DisableModsCommandHandler(
        IApplicationPersistenceContext applicationPersistenceContext,
        IPhysicalFileService physicalFileService
    )
    {
        _applicationPersistenceContext = applicationPersistenceContext;
        _physicalFileService = physicalFileService;
    }
    
    public Task<List<Domain.Entities.ModSettings>> Handle(DisableModsCommand request, CancellationToken cancellationToken)
    {
        var gameSettings = _applicationPersistenceContext.GetGameSettings(request.Game);
        
        foreach (var modSettings in request.ModsSettings)
        {
            if (_physicalFileService.IsModeEnabled(gameSettings,  modSettings))
                _physicalFileService.DisableMod(gameSettings, modSettings);
            
            modSettings.Enabled = false;
            _applicationPersistenceContext.Save(modSettings);
        }
        
        return Task.FromResult(_applicationPersistenceContext.GetModsSettings(request.Game));
    }
}