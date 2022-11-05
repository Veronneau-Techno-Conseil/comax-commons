import os
from jinja2 import Environment, PackageLoader, select_autoescape

print(os.getcwd())

def getText(path):
    f = open(path, "r")
    retVal = f.read().strip(' \n\r')
    print(retVal)
    f.close()
    return retVal

version = getText("./VERSION")
chartVersion = getText("./helm/VERSION")

env = Environment(
    loader=PackageLoader("model"),
    autoescape=select_autoescape()
)

template = env.get_template("Chart.yaml.jinja")

tgt = open("./helm/Chart.yaml", "w")
tgt.write(template.render(version=version, chartVersion=chartVersion))
tgt.close()
